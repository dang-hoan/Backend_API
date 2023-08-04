using Application.Interfaces.View.ViewCustomerBookingHistory;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.View.ViewCustomerBookingHistory
{
    public class ViewCustomerBookingHistoryRepository : RepositoryAsync<Domain.Entities.View.ViewCustomerBookingHistory.ViewCustomerBookingHistory, long>, IViewCustomerBookingHistoryRepository
    {
        public ViewCustomerBookingHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}