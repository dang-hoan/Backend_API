using Application.Interfaces.Booking;
using Infrastructure.Contexts;
namespace Infrastructure.Repositories.Booking
{
    public class BookingRepository : RepositoryAsync<Domain.Entities.Booking.Booking, long>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
