using Application.Interfaces.BookingDetail;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.BookingDetail
{
    public class BookingDetailRepository : RepositoryAsync<Domain.Entities.BookingDetail.BookingDetail, long>, IBookingDetailRepository
    {
        public BookingDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
