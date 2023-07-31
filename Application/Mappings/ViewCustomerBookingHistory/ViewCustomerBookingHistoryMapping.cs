using Application.Features.Booking.Queries.GetCustomerBookingHistory;
using AutoMapper;

namespace Application.Mappings.ViewCustomerBookingHistory
{
    public class ViewCustomerBookingHistoryMapping : Profile
    {
        public ViewCustomerBookingHistoryMapping() { 
            CreateMap<Domain.Entities.View.ViewCustomerBookingHistory.ViewCustomerBookingHistory, GetCustomerBookingHistoryResponse>().ReverseMap();
        }
    }
}
