using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
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


        public EditCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _customnerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditCustomerCommand>> Handle(EditCustomerCommand request, CancellationToken cancellationToken)
        {
            if(request.Id == 0)
            {
                return await Result<EditCustomerCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
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
