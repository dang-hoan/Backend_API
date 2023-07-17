using Application.Interfaces.Booking;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Booking
{
    public class BookingRepository : RepositoryAsync<Domain.Entities.Booking.Booking, long>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
