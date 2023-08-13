using Application.Interfaces.Booking;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Domain.Constants.Enum;
using System.Net.WebSockets;
using SkiaSharp;
using Domain.Constants;
using System.Reflection;
using Application.Interfaces.Customer;

namespace Application.Features.Booking.Command.UpdateStatusBooking
{
    public class UpdateStatusBookingCommand : IRequest<Result<UpdateStatusBookingCommand>>
    {
        public long Id { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }

    internal class EditBookingCommandHandler : IRequestHandler<UpdateStatusBookingCommand, Result<UpdateStatusBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICustomerRepository _customerRepository;

        public EditBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository, IUnitOfWork<long> unitOfWork, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task<Result<UpdateStatusBookingCommand>> Handle(UpdateStatusBookingCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(typeof(BookingStatus), request.BookingStatus)) return await Result<UpdateStatusBookingCommand>.FailAsync("Status is not exist");
            var ExistBooking = await _bookingRepository.FindAsync(x => !x.IsDeleted && x.Id == request.Id) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_BOOKING);
            if (ExistBooking.Status == request.BookingStatus) return await Result<UpdateStatusBookingCommand>.SuccessAsync();
            var temp = ExistBooking.Status;
            ExistBooking.Status = request.BookingStatus;
            await _bookingRepository.UpdateAsync(ExistBooking);
            await _unitOfWork.Commit(cancellationToken);
            if(request.BookingStatus == BookingStatus.Done || temp == BookingStatus.Done)
            {
                var ExistCustomer = await _customerRepository.FindAsync(x => !x.IsDeleted && x.Id == ExistBooking.CustomerId);
                if(ExistCustomer != null)
                {
                    ExistCustomer.TotalMoney = _bookingRepository.GetAllTotalMoneyBookingByCustomerId(ExistCustomer.Id);
                    await _customerRepository.UpdateAsync(ExistCustomer);
                    await _unitOfWork.Commit(cancellationToken);
                }
            }
            return await Result<UpdateStatusBookingCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
