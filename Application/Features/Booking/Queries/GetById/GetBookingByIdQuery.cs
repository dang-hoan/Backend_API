using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdQuery : IRequest<Result<GetBookingByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Result<GetBookingByIdResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetBookingByIdQueryHandler(IBookingRepository bookingRepository, IBookingDetailRepository bookingDetailRepository,
            IServiceRepository serviceRepository, IMapper mapper, ICustomerRepository customerRepository, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<Result<GetBookingByIdResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var Booking = await _bookingRepository.Entities
                .Where(_ => _.Id == request.Id && !_.IsDeleted)
                .Select(s => new Domain.Entities.Booking.Booking
                {
                    Id = s.Id,
                    BookingDate = s.BookingDate,
                    Status = s.Status,
                    FromTime = s.FromTime,
                    ToTime = s.ToTime,
                    Note = s.Note,  
                    CustomerId = s.CustomerId
                }).FirstOrDefaultAsync();
            if(Booking == null)
            {
                return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            {
                long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                if (userId != Booking.CustomerId)
                    return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            }

            var BookingDetailResponse = _mapper.Map<GetBookingByIdResponse>(Booking);
            var CustomerBooking = await _customerRepository.Entities
                .Where(_ => _.Id == Booking.CustomerId)
                .Select(s => new Domain.Entities.Customer.Customer
                {
                    Id = s.Id,
                    CustomerName = s.CustomerName,
                    PhoneNumber = s.PhoneNumber,
                }).FirstOrDefaultAsync();
            if(CustomerBooking == null)
            {
                return await Result<GetBookingByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            BookingDetailResponse.CustomerName = CustomerBooking.CustomerName;
            BookingDetailResponse.PhoneNumber = CustomerBooking.PhoneNumber;
            var BookingDetails = await _bookingDetailRepository.Entities
                .Where(_ => _.BookingId == Booking.Id && !_.IsDeleted)
                .Select(s => new Domain.Entities.BookingDetail.BookingDetail
                {
                    ServiceId = s.ServiceId,
                    BookingId = s.BookingId,
                }).ToListAsync();
            foreach(var bookingDetail in BookingDetails)
            {
                var Service = await _serviceRepository.Entities
                    .Where(_ => _.Id == bookingDetail.ServiceId)
                    .Select(s => new Domain.Entities.Service.Service
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        ServiceTime = s.ServiceTime,
                    }).FirstOrDefaultAsync();
                if(Service != null)
                {
                    BookingDetailResponse.Services.Add(new ServiceBookingResponse
                    {
                        Id = Service.Id,
                        Name = Service.Name,
                        Price = Service.Price,
                        ServiceTime = Service.ServiceTime,
                    });
                }
            }
            return await Result<GetBookingByIdResponse>.SuccessAsync(BookingDetailResponse);
        }
    }
}
