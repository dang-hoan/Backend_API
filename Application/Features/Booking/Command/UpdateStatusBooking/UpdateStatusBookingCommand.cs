using Application.Interfaces.Booking;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Domain.Constants;
using Application.Interfaces.Customer;
using Application.Interfaces;

namespace Application.Features.Booking.Command.UpdateStatusBooking
{
    public class UpdateStatusBookingCommand : IRequest<Result<UpdateStatusBookingCommand>>
    {
        public long Id { get; set; }
        public int BookingStatus { get; set; }
    }

    internal class EditBookingCommandHandler : IRequestHandler<UpdateStatusBookingCommand, Result<UpdateStatusBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICustomerRepository _customerRepository;

        public EditBookingCommandHandler(
            IMapper mapper, 
            IBookingRepository bookingRepository,
            IEnumService enumService,
            IUnitOfWork<long> unitOfWork,
            ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task<Result<UpdateStatusBookingCommand>> Handle(UpdateStatusBookingCommand request, CancellationToken cancellationToken)
        {
            var ExistBooking = await _bookingRepository.FindAsync(x => !x.IsDeleted && x.Id == request.Id) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_BOOKING);

            if (!_enumService.CheckEnumExistsById(request.BookingStatus, StaticVariable.BOOKING_STATUS_ENUM)) return await Result<UpdateStatusBookingCommand>.FailAsync(StaticVariable.STATUS_NOT_EXIST);

            if (ExistBooking.Status == request.BookingStatus) return await Result<UpdateStatusBookingCommand>.SuccessAsync(StaticVariable.SUCCESS);
            var temp = ExistBooking.Status;
            ExistBooking.Status = request.BookingStatus;
            await _bookingRepository.UpdateAsync(ExistBooking);
            await _unitOfWork.Commit(cancellationToken);
            if(request.BookingStatus == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM) || temp == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
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
