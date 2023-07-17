using Application.Interfaces.BookingDetail;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.BookingDetail
{
    public class BookingDetailRepository : RepositoryAsync<Domain.Entities.BookingDetail.BookingDetail, long>, IBookingDetailRepository
    {
        public BookingDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
