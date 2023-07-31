using Application.Interfaces.Repositories;

namespace Application.Interfaces.Booking
{
    public interface IBookingRepository : IRepositoryAsync<Domain.Entities.Booking.Booking, long>
    {
        decimal GetAllTotalMoneyBookingByCustomerId(long id);
    }
}
