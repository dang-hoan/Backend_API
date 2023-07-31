using Application.Interfaces.View.ViewCustomerBookingHistory;
using AutoMapper;
using Domain.Entities.View.ViewCustomerBookingHistory;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Queries.GetCustomerBookingHistory
{
    public class GetCustomerBookingHistoryQuery : IRequest<Result<List<GetCustomerBookingHistoryResponse>>>
    {
        public long CustomerId { get; set; }
    }
    internal class GetCustomerBookingHistoryQueryHandler : IRequestHandler<GetCustomerBookingHistoryQuery, Result<List<GetCustomerBookingHistoryResponse>>>
    {
        private readonly IViewCustomerBookingHistoryRepository _viewCustomerBookingHistoryRepository;
        private readonly IMapper _mapper;
        public GetCustomerBookingHistoryQueryHandler(IViewCustomerBookingHistoryRepository viewCustomerBookingHistoryRepository, IMapper mapper)
        {
            _viewCustomerBookingHistoryRepository = viewCustomerBookingHistoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCustomerBookingHistoryResponse>>> Handle(GetCustomerBookingHistoryQuery request, CancellationToken cancellationToken)
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
            var historyResponse = _mapper.Map<List<GetCustomerBookingHistoryResponse>>(history);
            return await Result<List<GetCustomerBookingHistoryResponse>>.SuccessAsync(historyResponse);
        }
    }
}