using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Entities.BookingDetail;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.EditBooking
{
    public class EditBookingCommand : IRequest<Result<EditBookingCommand>>
    {
        public long Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
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
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public EditBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository, ICustomerRepository customerRepository, 
            IServiceRepository serviceRepository, IUnitOfWork<long> unitOfWork, IBookingDetailRepository bookingDetailService,
            ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _serviceRepository = serviceRepository;
            _bookingDetailRepository = bookingDetailService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<EditBookingCommand>> Handle(EditBookingCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                request.ServiceId = request.ServiceId.Distinct().ToList();
                if (request.ToTime.CompareTo(request.FromTime) < 0)
                {
                    return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_LOGIC_DATE_ORDER);
                }
                var isExistBooking = await _bookingRepository.FindAsync(_ => _.Id == request.Id && _.IsDeleted == false);
                if (isExistBooking == null) return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_BOOKING);

                if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
                {
                    long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                    if (userId != isExistBooking.CustomerId)
                        return await Result<EditBookingCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
                }

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
                await _unitOfWork.Commit(cancellationToken);

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