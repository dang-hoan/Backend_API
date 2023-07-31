using Application.Interfaces.View.ViewCustomerBookingHistory;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.View.ViewCustomerBookingHistory
{
    public class ViewCustomerBookingHistoryRepository : RepositoryAsync<Domain.Entities.View.ViewCustomerBookingHistory.ViewCustomerBookingHistory, long>, IViewCustomerBookingHistoryRepository
    {
        public ViewCustomerBookingHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
