using Application.Interfaces.View.ViewCustomerReviewHistory;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.View.ViewCustomerReviewHistory
{
    public class ViewCustomerReviewHistoryRepository : RepositoryAsync<Domain.Entities.View.ViewCustomerReviewHistory.ViewCustomerReviewHistory, long>, IViewCustomerReviewHisotyRepository
    {
        public ViewCustomerReviewHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}