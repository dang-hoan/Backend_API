using Application.Features.Employee.Queries.GetById;
using Application.Interfaces.Booking;
using Application.Interfaces.View.ViewCustomerBookingHistory;
using AutoMapper;
using Domain.Entities.View.ViewCustomerBookingHistory;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.util;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Features.Booking.Queries.GetCustomerBookingHistory
{
    public class GetCustomerBookingHistoryQuery : IRequest<Result<GetCustomerBookingHistoryResponse>>
    {
        public long CustomerId { get; set; }
    }
    internal class GetCustomerBookingHistoryQueryHandler : IRequestHandler<GetCustomerBookingHistoryQuery, Result<GetCustomerBookingHistoryResponse>>
    {
        private readonly IViewCustomerBookingHistoryRepository _viewCustomerBookingHistoryRepository;
        private readonly IMapper _mapper;
        public GetCustomerBookingHistoryQueryHandler(IViewCustomerBookingHistoryRepository viewCustomerBookingHistoryRepository, IMapper mapper)
        {
            _viewCustomerBookingHistoryRepository = viewCustomerBookingHistoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetCustomerBookingHistoryResponse>> Handle(GetCustomerBookingHistoryQuery request, CancellationToken cancellationToken)
        {
            var history = await _viewCustomerBookingHistoryRepository.Entities
                .Where(_ => _.CustomerId == request.CustomerId)
                .Select(s => new ViewCustomerBookingHistory
                {
                    CustomerId = s.CustomerId,
                    BookingId = s.BookingId,
                    BookingDate = s.BookingDate,
                    FromTime = s.FromTime,
                    ToTime = s.ToTime,
                    Status = s.Status,
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Price = s.Price,
                }).ToListAsync();
            GetCustomerBookingHistoryResponse historyResponse = new GetCustomerBookingHistoryResponse();
            var historyConvertData = _mapper.Map<List<CustomerBookingHistoryResponse>>(history);
            if (historyConvertData != null)
            {
                foreach (CustomerBookingHistoryResponse customerBookingHistory in historyConvertData)
                {
                    historyResponse.CustomerBookingHistorys.Add(customerBookingHistory);
                }
            }
            return await Result<GetCustomerBookingHistoryResponse>.SuccessAsync(historyResponse);
        }
    }
}