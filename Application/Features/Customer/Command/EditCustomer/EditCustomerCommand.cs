﻿using Application.Interfaces;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Application.Features.Customer.Command.EditCustomer
{
    public class EditCustomerCommand : IRequest<Result<EditCustomerCommand>>
    {
        public long Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
 
        [RegularExpression(@"(\+84|84|0)+(3|5|7|8|9|1[2|6|8|9])+([0-9]{7,8})\b", ErrorMessage = StaticVariable.INVALID_PHONE_NUMBER)]
        public string PhoneNumber { get; set; }
    }

    internal class EditCustomerCommandHandler : IRequestHandler<EditCustomerCommand, Result<EditCustomerCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customnerRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public EditCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository, IUnitOfWork<long> unitOfWork, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _customnerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<EditCustomerCommand>> Handle(EditCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditCustomerCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            {
                long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                if (userId != request.Id)
                    return await Result<EditCustomerCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            }

            var editCustomer = await _customnerRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editCustomer == null) return await Result<EditCustomerCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            _mapper.Map(request, editCustomer);
            var errorLimitCharacter = StringHelper.CheckLimitCustomer(editCustomer);
            if (!errorLimitCharacter.Equals(""))
            {
                return await Result<EditCustomerCommand>.FailAsync(errorLimitCharacter);
            }
            bool isPhoneNumberExists = _customnerRepository.Entities.Any(x => x.PhoneNumber.Equals(request.PhoneNumber) && x.Id != request.Id && !x.IsDeleted);
            if (isPhoneNumberExists)
            {
                return await Result<EditCustomerCommand>.FailAsync(StaticVariable.PHONE_NUMBER_EXISTS_MSG);
            }
            await _customnerRepository.UpdateAsync(editCustomer);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = editCustomer.Id;
            return await Result<EditCustomerCommand>.SuccessAsync(request);
        }
    }
}
