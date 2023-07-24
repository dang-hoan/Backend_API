using Application.Interfaces.Booking;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Booking
{
    public class BookingRepository : RepositoryAsync<Domain.Entities.Booking.Booking, long>, IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public decimal GetAllTotalMoneyBookingByCustomerId(long id)
        {
            var totalMoney = _dbContext.Bookings.Where(b => b.CustomerId == id && !b.IsDeleted)
               .Join(_dbContext.BookingDetails.Where(_ => !_.IsDeleted), b => b.Id, bd => bd.BookingId, (b, bd) => new { Booking = b, BookingDetail = bd })
             .Join(_dbContext.Services.Where(_ => !_.IsDeleted), bb => bb.BookingDetail.ServiceId, s => s.Id, (bb, s) => new { Booking = bb.Booking, Service = s })
             .GroupBy(bb => bb.Booking.CustomerId)
            .Select(group => new
            {
                CustomerId = group.Key,
                TotalMoney = group.Sum(bb => bb.Service.Price)
            })
            .FirstOrDefault(); ;
            return (totalMoney != null) ?  totalMoney.TotalMoney : 0;
        }
    }
}
