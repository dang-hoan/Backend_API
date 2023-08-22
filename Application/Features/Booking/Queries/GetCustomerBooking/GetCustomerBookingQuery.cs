using Application.Dtos.Responses.ServiceImage;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Parameters;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : RequestParameter, IRequest<PaginatedResult<GetCustomerBookingResponse>>
    {
        [Required]
        public long CustomerId { get; set; }

        public int? BookingStatus { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, PaginatedResult<GetCustomerBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IEnumService _enumService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetCustomerBookingQueryHandler(IBookingRepository bookingRepository, IBookingDetailRepository bookingDetailRepository, IServiceRepository serviceRepository,
            IServiceImageRepository serviceImageRepository, IMapper mapper, IEnumService enumService, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _enumService = enumService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<PaginatedResult<GetCustomerBookingResponse>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

            if (userId != request.CustomerId)
                throw new ApiException(StaticVariable.NOT_HAVE_ACCESS);

            if (request.BookingStatus != null && !_enumService.CheckEnumExistsById((int)request.BookingStatus))
                throw new ApiException(StaticVariable.STATUS_NOT_EXIST);

            DateTime findedDate;

            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var customerBooking = from booking in _bookingRepository.Entities.AsEnumerable()
                                  let check = 0
                                  where !booking.IsDeleted && booking.CustomerId == request.CustomerId
                                        && (request.BookingStatus == null || booking.Status == request.BookingStatus)
                                  select new GetCustomerBookingResponse
                                  {
                                      BookingId = booking.Id,
                                      BookingDate = booking.BookingDate,
                                      FromTime = booking.FromTime,
                                      ToTime = booking.ToTime,
                                      BookingStatus = booking.Status,
                                      CreatedOn = booking.CreatedOn,
                                      LastModifiedOn = booking.LastModifiedOn,
                                      bookingDetailResponses = (from bookingDetail in _bookingDetailRepository.Entities.AsEnumerable()
                                                                join service in _serviceRepository.Entities.AsEnumerable() on bookingDetail.ServiceId equals service.Id
                                                                where booking.Id == bookingDetail.BookingId && !bookingDetail.IsDeleted && !service.IsDeleted
                                                                select new BookingDetailResponse
                                                                {
                                                                    BookingDetailId = bookingDetail.Id,
                                                                    ServiceId = bookingDetail.ServiceId,
                                                                    ServiceDescription = service.Description,
                                                                    ServiceImages = _mapper.Map<List<ServiceImageResponse>>(_serviceImageRepository.Entities.Where(_ => _.ServiceId == service.Id && _.IsDeleted == false).ToList()),
                                                                    ServiceName = service.Name,
                                                                    ServicePrice = service.Price
                                                                }).ToList()
                                  };

            if(!string.IsNullOrEmpty(request.Keyword))
                customerBooking = customerBooking.Where(
                    x => x.BookingId.ToString().Contains(request.Keyword)
                            || DateTime.TryParse(request.Keyword, out findedDate) && x.BookingDate.ToString("yyyy-MM-dd").Equals(findedDate.ToString("yyyy-MM-dd"))          
                            || x.bookingDetailResponses.Where(x => StringHelper.Contains(x.ServiceName, request.Keyword)).Any());

            var data = customerBooking.AsQueryable().OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetCustomerBookingResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetCustomerBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);

        }
    }
}