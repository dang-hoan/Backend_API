using Application.Interfaces;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
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

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]\d{7,9}$", ErrorMessage = "Phone number must be between 8 and 10 digits.")]
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
            if(request.Id == 0)
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
            if(editCustomer == null) return await Result<EditCustomerCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            _mapper.Map(request, editCustomer);
            await _customnerRepository.UpdateAsync(editCustomer);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = editCustomer.Id;
            return await Result<EditCustomerCommand>.SuccessAsync(request);
        }
    }
}
