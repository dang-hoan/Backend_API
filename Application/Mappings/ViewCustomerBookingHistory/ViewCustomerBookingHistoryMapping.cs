using Application.Features.Customer.Queries.GetCustomerBookingHistory;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.ViewCustomerBookingHistory
{
    public class ViewCustomerBookingHistoryMapping : Profile
    {
        public ViewCustomerBookingHistoryMapping() { 
            CreateMap<Domain.Entities.ViewBookingHistory.ViewCustomerBookingHistory, CustomerBookingHistoryResponse>().ReverseMap();
        }
    }
}
