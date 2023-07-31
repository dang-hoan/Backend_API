using Application.Interfaces.Repositories;

namespace Application.Interfaces.BookingDetail
{
    public interface IBookingDetailRepository : IRepositoryAsync<Domain.Entities.BookingDetail.BookingDetail, long>
    {
    }
}
