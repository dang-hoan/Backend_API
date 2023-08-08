﻿using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingQuery : IRequest<Result<List<GetCustomerBookingResponse>>>
    {
        [Required]
        public long CustomerId { get; set; }

        public string? KeyWord { get; set; }
    }

    internal class GetCustomerBookingQueryHandler : IRequestHandler<GetCustomerBookingQuery, Result<List<GetCustomerBookingResponse>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;

        public GetCustomerBookingQueryHandler(IBookingRepository bookingRepository, IBookingDetailRepository bookingDetailRepository, IServiceRepository serviceRepository,
            IServiceImageRepository serviceImageRepository, IMapper mapper, IUploadService uploadService)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _uploadService = uploadService;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCustomerBookingResponse>>> Handle(GetCustomerBookingQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.Entities.Where(_ => !_.IsDeleted && _.CustomerId == request.CustomerId)
                .Select(s => new Domain.Entities.Booking.Booking
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    BookingDate = s.BookingDate,
                    FromTime = s.FromTime,
                    ToTime = s.ToTime,
                    LastModifiedOn = s.LastModifiedOn,
                    Status = s.Status,
                    Note = s.Note,
                }).ToListAsync();
            List<GetCustomerBookingResponse> response = new List<GetCustomerBookingResponse>();
            foreach (var booking in bookings)
            {
                var bookingResponse = new GetCustomerBookingResponse
                {
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
                foreach (var bookingDetail in bookingDetails)
                {
                    var service = await _serviceRepository.FindAsync(_ => !_.IsDeleted && _.Id == bookingDetail.ServiceId);
                    if (service != null)
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
                            imageResponse.NameFile = _uploadService.GetFullUrl(imageResponse.NameFile);
                        }
                        bookingDetailResponses.Add(bookingDetailResponse);

                        if (request.KeyWord != null)
                            request.KeyWord = request.KeyWord.Trim();

                        if (string.IsNullOrEmpty(request.KeyWord) || StringHelper.Contains(service.Name, request.KeyWord)
                            || booking.Id.ToString().Contains(request.KeyWord) || booking.BookingDate.ToString("dd/MM/yyyy").Contains(request.KeyWord)
                            || booking.BookingDate.ToString("dd-MM-yyyy").Contains(request.KeyWord))
                        {
                            checkServiceName = true;
                        }
                    }
                }
                if (checkServiceName)
                {
                    bookingResponse.bookingDetailResponses.AddRange(bookingDetailResponses);
                    response.Add(bookingResponse);
                }
            }
            return await Result<List<GetCustomerBookingResponse>>.SuccessAsync(response);
        }
    }
}