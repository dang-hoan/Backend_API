using Application.Interfaces.Repositories;
using Domain.Entities.ViewBookingHistory;

namespace Application.Interfaces.ViewCustomerBookingHistory
{
    public interface IViewCustomerBookingHistoryRepository : IRepositoryAsync<Domain.Entities.ViewBookingHistory.ViewCustomerBookingHistory,long>
    {
    }
}
