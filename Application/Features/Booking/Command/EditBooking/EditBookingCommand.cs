using Application.Exceptions;
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
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.EditBooking
{
    public class EditBookingCommand : IRequest<Result<EditBookingCommand>>
    {
        public long Id { get; set; }
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
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                request.ServiceId = request.ServiceId.Distinct().ToList();
                if (request.Totime.CompareTo(request.FromTime) < 0)
                {
                    return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_LOGIC_DATE_ORDER);
                }
                var isExistBooking = await _bookingRepository.FindAsync(_ => _.Id == request.Id && _.IsDeleted == false);
                if (isExistBooking == null) return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING);
                var customer = await _customerRepository.FindAsync(_ => _.Id == isExistBooking.CustomerId && !_.IsDeleted);
                if (customer == null) return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

                _mapper.Map(request, isExistBooking);

                // check request.ServiceId exist in db
                List<long> listExistServiceId = _serviceRepository.Entities.Where(_ => !_.IsDeleted).Select(_ => _.Id).ToList();
                if (request.ServiceId.Except(listExistServiceId).ToList().Any()) return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_SERVICE);

                List<long> existingServiceIds = _bookingDetailRepository.Entities.Where(x => !x.IsDeleted && x.BookingId == request.Id).Select(b => b.ServiceId).ToList();
                List<long> serviceIdToDelete = existingServiceIds.Except(request.ServiceId).ToList();
                List<BookingDetail> bookingServiceIsDelete = await _bookingDetailRepository.Entities.Where(x => serviceIdToDelete.Contains(x.ServiceId) && x.BookingId == request.Id && !x.IsDeleted).ToListAsync();
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

                customer.TotalMoney = _bookingRepository.GetAllTotalMoneyBookingByCustomerId(isExistBooking.CustomerId);
                await _customerRepository.UpdateAsync(customer);

                await transaction.CommitAsync(cancellationToken);
                return await Result<EditBookingCommand>.SuccessAsync(request);
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