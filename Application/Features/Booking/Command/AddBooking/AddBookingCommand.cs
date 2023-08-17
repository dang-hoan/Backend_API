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
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command.AddBooking
{
    public class AddBookingCommand : IRequest<Result<AddBookingCommand>>
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string? Note { get; set; }
        public List<long> ServiceId { get; set; }
    }

    internal class AddBookingCommandHandler : IRequestHandler<AddBookingCommand, Result<AddBookingCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEnumService _enumService;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IBookingDetailRepository _bookingDetailService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public AddBookingCommandHandler(IMapper mapper, IBookingRepository bookingRepository, IServiceRepository serviceRepository, IEnumService enumService, IUnitOfWork<long> unitOfWork, 
            IBookingDetailRepository bookingDetailService, ICustomerRepository customerRepository, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _enumService = enumService;
            _unitOfWork = unitOfWork;
            _serviceRepository = serviceRepository;
            _bookingDetailService = bookingDetailService;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<AddBookingCommand>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            {
                long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                if (userId != request.CustomerId)
                    return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            }

            //open transaction
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                request.ServiceId = request.ServiceId.Distinct().ToList();
                if (request.ToTime.CompareTo(request.FromTime) < 0)
                {
                    return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_LOGIC_DATE_ORDER);
                }
                var ExistCustomer = await _customerRepository.FindAsync(x => x.Id == request.CustomerId && !x.IsDeleted);
                if (ExistCustomer == null) return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_CUSTOMER);

                // check request.ServiceId exist in db
                List<long> listExistServiceId = await _serviceRepository.Entities.Where(x => !x.IsDeleted).Select(x => x.Id).ToListAsync();
                if (request.ServiceId.Except(listExistServiceId).ToList().Any()) return await Result<AddBookingCommand>.FailAsync(StaticVariable.NOT_FOUND_SERVICE);

                var booking = _mapper.Map<Domain.Entities.Booking.Booking>(request);
                booking.Status = _enumService.GetEnumIdByValue(StaticVariable.WAITING, StaticVariable.BOOKING_STATUS_ENUM);
                await _bookingRepository.AddAsync(booking);
                await _unitOfWork.Commit(cancellationToken);
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
                await _unitOfWork.Commit(cancellationToken);
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