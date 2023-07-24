using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : IRequest<Result<GetCustomerBookingResponse>>
    {
        [Required]
        public long CustomerId { get; set; }
        public string? KeyWord { get; set; }
    }
    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, Result<GetCustomerBookingResponse>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IUploadService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetCustomerBookingQueryHandler(IBookingRepository bookingRepository, IBookingDetailRepository bookingDetailRepository, IServiceRepository serviceRepository,
            IServiceImageRepository serviceImageRepository, IMapper mapper, IUploadService uploadService, IHttpContextAccessor httpContextAccessor)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _uploadService = uploadService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<GetCustomerBookingResponse>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.Entities.Where(_ => !_.IsDeleted && _.CustomerId == request.CustomerId)
                .Select(s => new Domain.Entities.Booking.Booking
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    BookingDate = s.BookingDate,
                    FromTime = s.FromTime,
                    Totime = s.Totime,
                    LastModifiedOn = s.LastModifiedOn,
                    Status = s.Status,
                    Note = s.Note,
                }).ToListAsync();
            GetCustomerBookingResponse response = new GetCustomerBookingResponse();
            foreach(var booking in bookings)
            {
                var bookingResponse = new BookingResponse { 
                    BookingId = booking.Id,
                    BookingDate = booking.BookingDate,
                    BookingStatus = booking.Status,
                    LastModifiedOn = booking.LastModifiedOn,
                };
                var bookingDetails = await _bookingDetailRepository.Entities.Where(_ => !_.IsDeleted && _.BookingId == booking.Id)
                    .Select(s => new Domain.Entities.BookingDetail.BookingDetail
                    {
                        Id = s.Id,
                        BookingId = booking.Id,
                        ServiceId = s.ServiceId,
                        Note = s.Note,
                    }).ToListAsync();
                bool checkServiceName = false;
                List<BookingDetailResponse> bookingDetailResponses = new List<BookingDetailResponse>();
                foreach(var bookingDetail in bookingDetails)
                {
                    var service = await _serviceRepository.FindAsync(_ => !_.IsDeleted && _.Id == bookingDetail.ServiceId);
                    if(service != null)
                    {
                        var bookingDetailResponse = new BookingDetailResponse
                        {
                            BookingDetailId = bookingDetail.Id,
                            ServiceId = bookingDetail.ServiceId,
                            ServiceDescription = service.Description,
                            ServiceImages = _mapper.Map<List<ServiceImageResponse>>(_serviceImageRepository.Entities.Where(_ => _.ServiceId == service.Id && _.IsDeleted == false).ToList()),
                            ServiceName = service.Name,
                            ServicePrice = service.Price,
                        };
                        foreach (ServiceImageResponse imageResponse in bookingDetailResponse.ServiceImages)
                        {
                            imageResponse.NameFile = _uploadService.GetImageLink(imageResponse.NameFile, _httpContextAccessor);
                        }
                        bookingDetailResponses.Add(bookingDetailResponse);
                        if (string.IsNullOrEmpty(request.KeyWord) || service.Name.ToLower().Contains(request.KeyWord.ToLower()) || booking.Id.ToString().Contains(request.KeyWord) || booking.BookingDate.ToString("dd/MM/yyyy").Contains(request.KeyWord) || booking.BookingDate.ToString("dd-MM-yyyy").Contains(request.KeyWord))
                        {
                            checkServiceName = true;
                        }
                    }
                }
                if (checkServiceName)
                {
                    bookingResponse.bookingDetailResponses.AddRange(bookingDetailResponses);
                    response.bookings.Add(bookingResponse);
                }
            }
            return await Result<GetCustomerBookingResponse>.SuccessAsync(response);
        }
    }
}
