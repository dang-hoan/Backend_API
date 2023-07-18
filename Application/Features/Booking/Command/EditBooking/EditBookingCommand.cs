using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.BookingDetail;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Booking.Command.EditBooking
{
    public class EditBookingCommand : IRequest<Result<EditBookingCommand>>
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime Totime { get; set; }
        public string? Note { get; set; }
        public List<long> ServiceId { get; set; }
    }
    internal class EditBookingCommandHandler : IRequestHandler<EditBookingCommand, Result<EditBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingDetailRepository _bookingDetailRepository;


        public EditBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository, ICustomerRepository customerRepository, IServiceRepository serviceRepository, IUnitOfWork<long> unitOfWork, IBookingDetailRepository bookingDetailService)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _serviceRepository = serviceRepository;
            _bookingDetailRepository = bookingDetailService;
        }

        public async Task<Result<EditBookingCommand>> Handle(EditBookingCommand request, CancellationToken cancellationToken)
        {
            if (request.Totime.CompareTo(request.FromTime) < 0)
            {
                return await Result<EditBookingCommand>.FailAsync("ToTime must be greater than FromTime");
            }
            var isExistBooking = await _bookingRepository.FindAsync(_ => _.Id == request.Id && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_BOOKING);
            _mapper.Map(request, isExistBooking);
            var isExistCustomer = await _customerRepository.FindAsync(_ => _.Id == request.CustomerId && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_CUSTOMER);
            foreach (long i in request.ServiceId)
            {
                bool isExistedService = await IsExistedIdService(i);
                if (!isExistedService)
                {
                    throw new KeyNotFoundException(StaticVariable.NOT_FOUND_SERVICE);
                }
            }
            List<long> existingServiceIds = _bookingDetailRepository.Entities.Where(_ => _.IsDeleted == false && _.BookingId == request.Id)
                .Select(b => b.ServiceId).ToList();
            List<long> serviceIdToDelete = existingServiceIds.Except(request.ServiceId).ToList();
            List<BookingDetail> bookingServiceIsDelete = _bookingDetailRepository.Entities
                .Where(_ => serviceIdToDelete.Contains(_.ServiceId) && _.BookingId == request.Id && _.IsDeleted == false).ToList();
            foreach (BookingDetail i in bookingServiceIsDelete)
            {
                i.IsDeleted = true;
            }
            await _bookingDetailRepository.UpdateRangeAsync(bookingServiceIsDelete);
            List<long> serviceIdToAdd = request.ServiceId.Except(existingServiceIds).ToList();
            List<BookingDetail> bookingDetailsToAdd = serviceIdToAdd.Select(serviceId => new BookingDetail
            {
                BookingId = request.Id,
                ServiceId = serviceId,
                Note = request.Note
            }).ToList();
            await _bookingDetailRepository.AddRangeAsync(bookingDetailsToAdd);
            await _bookingRepository.UpdateAsync(isExistBooking);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<EditBookingCommand>.SuccessAsync(request, "Success");
        }
                
        public async Task<bool> IsExistedIdService(long id)
        {
            var isExistedId = await _serviceRepository.FindAsync(_ => _.Id == id && _.IsDeleted == false);
            return isExistedId != null;
        }
    }
}
