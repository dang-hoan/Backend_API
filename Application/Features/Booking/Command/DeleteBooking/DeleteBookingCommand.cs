using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
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

        public DeleteBookingCommandHandler(
            IUnitOfWork<long> unitOfWork,
            IBookingRepository bookingRepository,
            IBookingDetailRepository bookingDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
        }

        public async Task<Result<long>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {

            var query = from b in _bookingRepository.Entities
                        join bd in _bookingDetailRepository.Entities on b.Id equals bd.BookingId
                        where !b.IsDeleted && b.Id == request.Id
                        select new
                        {
                            Status = b.Status,
                        };
            var booking = await _bookingRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);

            try
            {
                bool canDelete = true;
                foreach (var q in query)
                {
                    if (q.Status == BookingStatus.Inprogress)
                    {
                        canDelete = false;
                        break;
                    }
                }

                if (canDelete)
                {
                    var bookingDetail = await _bookingDetailRepository.GetByCondition(x => x.BookingId == request.Id && !x.IsDeleted) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);

                    await _bookingRepository.DeleteAsync(booking);

                    await _bookingDetailRepository.DeleteRange(bookingDetail.ToList());

                    await _unitOfWork.Commit(cancellationToken);

                    return await Result<long>.SuccessAsync(request.Id, $"Delete booking and booking detail by booking id {request.Id}  successfully!");
                }

                return await Result<long>.FailAsync("Booking is inprogress");
            }
            catch (System.Exception e)
            {
                return await Result<long>.FailAsync(e.Message);
            }
        }
    }
}