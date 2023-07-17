using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.BookingDetail
{
    public interface IBookingDetailRepository : IRepositoryAsync<Domain.Entities.BookingDetail.BookingDetail,long>
    {
    }
}
