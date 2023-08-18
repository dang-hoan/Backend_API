using Application.Features.Feedback.Queries.GetAll;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Customer;
using Application.Interfaces.Feedback;
using Application.Interfaces.Service;
using Domain.Constants;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetOustandingService
{
    public class GetOutstandingServiceQuery : IRequest<Result<List<GetOutstandingServiceResponse>>>
    {
    }
    internal class GetOutstandingServiceQueryHandler : IRequestHandler<GetOutstandingServiceQuery, Result<List<GetOutstandingServiceResponse>>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingDetailRepository _bookDetailRepository;
        private readonly IEnumService _enumService;
        private readonly IBookingRepository _bookingRepository;

        public GetOutstandingServiceQueryHandler(IServiceRepository serviceRepository, IBookingDetailRepository bookDetailRepository, IEnumService enumService,
            IBookingRepository bookingRepository            )
        {
            _serviceRepository = serviceRepository;
            _bookDetailRepository = bookDetailRepository;
            _enumService = enumService;
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<List<GetOutstandingServiceResponse>>> Handle(GetOutstandingServiceQuery request, CancellationToken cancellationToken)
        {
            List<GetOutstandingServiceResponse> response = new List<GetOutstandingServiceResponse>();
            var services = await _serviceRepository.GetAllAsync();
            if(services != null)
            {
                foreach(var service in services) {
                    GetOutstandingServiceResponse outstandingService = new GetOutstandingServiceResponse();
                    outstandingService.ServiceId = service.Id;
                    outstandingService.ServiceName = service.Name;

                    var bookingDetailServices = from bkd in _bookDetailRepository.Entities.AsEnumerable()
                                                join b in _bookingRepository.Entities.AsEnumerable() on bkd.BookingId equals b.Id
                                                where !b.IsDeleted && !bkd.IsDeleted
                                                && bkd.ServiceId == service.Id
                                                && b.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM)
                                                select new Domain.Entities.BookingDetail.BookingDetail
                                                {
                                                    Id = bkd.BookingId,
                                                    BookingId = bkd.BookingId,
                                                    ServiceId = bkd.ServiceId,
                                                };
                    outstandingService.Revenue = service.Price * bookingDetailServices.ToList().Count;
                    response.Add(outstandingService);
                }
                response = response.AsEnumerable().OrderByDescending(_ => _.Revenue).ToList();
            }
            return await Result<List<GetOutstandingServiceResponse>>.SuccessAsync(response);
        }
    }
}
