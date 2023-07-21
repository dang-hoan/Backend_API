using Application.Interfaces.View.ViewCustomerReviewHistory;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.View.ViewCustomerReviewHistory
{
    public class ViewCustomerReviewHistoryRepository : RepositoryAsync<Domain.Entities.View.ViewCustomerReviewHistory.ViewCustomerReviewHistory, long>, IViewCustomerReviewHisotyRepository
    {
        public ViewCustomerReviewHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
