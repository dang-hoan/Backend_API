using Application.Exceptions;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.AddBooking
{
    public class AddBookingCommand : IRequest<Result<AddBookingCommand>>
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime Totime { get; set; }
        public string? Note { get; set; }
        public List<long> ServiceId { get; set; }
    }

    internal class AddBookingCommandHandler : IRequestHandler<AddBookingCommand, Result<AddBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingDetailRepository _bookingDetailService;

        public AddBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository, IServiceRepository serviceRepository, IUnitOfWork<long> unitOfWork, IBookingDetailRepository bookingDetailService)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _serviceRepository = serviceRepository;
            _bookingDetailService = bookingDetailService;
        }

        public async Task<Result<AddBookingCommand>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
        {
            //open transaction
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                request.ServiceId = request.ServiceId.Distinct().ToList();
                if (request.Totime.CompareTo(request.FromTime) < 0)
                {
                    return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_LOGIC_DATE_ORDER);
                }

                // check request.ServiceId exist in db
                List<long> listExistServiceId = await _serviceRepository.Entities.Where(x => !x.IsDeleted).Select(x => x.Id).ToListAsync();
                if (request.ServiceId.Except(listExistServiceId).ToList().Any()) return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_SERVICE);

                var booking = _mapper.Map<Domain.Entities.Booking.Booking>(request);
                booking.Status = BookingStatus.Waiting;
                await _bookingRepository.AddAsync(booking);
                request.Id = booking.Id;
                foreach (long i in request.ServiceId)
                {
                    await _bookingDetailService.AddAsync(new Domain.Entities.BookingDetail.BookingDetail
                    {
                        BookingId = booking.Id,
                        ServiceId = i,
                        Note = booking.Note
                    });
                }

                await transaction.CommitAsync(cancellationToken);
                return await Result<AddBookingCommand>.SuccessAsync(request);
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