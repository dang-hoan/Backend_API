using Application.Exceptions;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Booking.Command.DeleteBooking
{
    public class DeleteBookingCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly ICustomerRepository _customerRepository;

        public DeleteBookingCommandHandler(
            IUnitOfWork<long> unitOfWork,
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IBookingDetailRepository bookingDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<long>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var booking = await _bookingRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if (booking == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
                if (!(booking.Status == BookingStatus.Inprogress))
                {
                    var bookingDetail = await _bookingDetailRepository.GetByCondition(x => x.BookingId == request.Id && !x.IsDeleted);
                    if (bookingDetail == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

                    await _bookingRepository.DeleteAsync(booking);

                    await _bookingDetailRepository.DeleteRange(bookingDetail.ToList());

                    await _unitOfWork.Commit(cancellationToken);

                    Domain.Entities.Customer.Customer ExistCustomer = await _customerRepository.FindAsync(_ => _.Id == booking.CustomerId && !_.IsDeleted);

                    if (ExistCustomer != null)
                    {
                        ExistCustomer.TotalMoney = _bookingRepository.GetAllTotalMoneyBookingByCustomerId(ExistCustomer.Id);
                        await _customerRepository.UpdateAsync(ExistCustomer);
                        await _unitOfWork.Commit(cancellationToken);
                    }
                    return await Result<long>.SuccessAsync(request.Id, $"Delete booking and booking detail by booking id {request.Id} successfully!");
                }
                await transaction.CommitAsync(cancellationToken);
                return await Result<long>.FailAsync("Booking is inprogress");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}