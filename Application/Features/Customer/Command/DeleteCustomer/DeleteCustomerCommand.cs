using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Customer.Command.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }

    }

    internal class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;

        public DeleteCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork, IBookingRepository bookingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var deleteCustomer = await _customerRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if(deleteCustomer == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            var query = from c in _customerRepository.Entities
                        join b in _bookingRepository.Entities on c.Id equals b.CustomerId
                        where !c.IsDeleted && !b.IsDeleted && c.Id == request.Id
                        select new
                        {
                            b.Status
                        };
            bool canDelete = true;

            foreach (var q in query)
            {
                if (q.Status == _enumService.GetEnumIdByValue(StaticVariable.INPROGRESSING, StaticVariable.BOOKING_STATUS_ENUM))
                {
                    canDelete = false;
                    break;
                }
            }

            if (canDelete)
            {
                await _customerRepository.DeleteAsync(deleteCustomer);
                await _unitOfWork.Commit(cancellationToken);
                
                return await Result<long>.SuccessAsync(request.Id, $"Delete customer by id {request.Id} successfully!");
            }

            return await Result<long>.FailAsync("Customer is using in other booking");
        }
    }
}