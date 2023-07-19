using Application.Interfaces.Repositories;

namespace Application.Interfaces.View.ViewCustomerBookingHistory
{
    public interface IViewCustomerBookingHistoryRepository : IRepositoryAsync<Domain.Entities.View.ViewCustomerBookingHistory.ViewCustomerBookingHistory, long>
    {
    }
}
