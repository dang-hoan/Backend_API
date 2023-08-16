using Application.Interfaces.Booking;
using Infrastructure.Contexts;
using Domain.Constants;
using Application.Interfaces;

namespace Infrastructure.Repositories.Booking
{
    public class BookingRepository : RepositoryAsync<Domain.Entities.Booking.Booking, long>, IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEnumService _enumService;

        public BookingRepository(ApplicationDbContext dbContext, IEnumService enumService) : base(dbContext)
        {
            _dbContext = dbContext;
            _enumService = enumService;
        }

        public decimal GetAllTotalMoneyBookingByCustomerId(long id)
        {
            var totalMoney = _dbContext.Bookings.Where(b => b.CustomerId == id && !b.IsDeleted && b.Status == _enumService.GetEnumIdByValue(StaticVariable.DONE, StaticVariable.BOOKING_STATUS_ENUM))
               .Join(_dbContext.BookingDetails.Where(_ => !_.IsDeleted), b => b.Id, bd => bd.BookingId, (b, bd) => new { Booking = b, BookingDetail = bd })
             .Join(_dbContext.Services.Where(_ => !_.IsDeleted), bb => bb.BookingDetail.ServiceId, s => s.Id, (bb, s) => new { Booking = bb.Booking, Service = s })
             .GroupBy(bb => bb.Booking.CustomerId)
            .Select(group => new
            {
                CustomerId = group.Key,
                TotalMoney = group.Sum(bb => bb.Service.Price)
            })
            .FirstOrDefault(); ;
            return (totalMoney != null) ? totalMoney.TotalMoney : 0;
        }
    }
}