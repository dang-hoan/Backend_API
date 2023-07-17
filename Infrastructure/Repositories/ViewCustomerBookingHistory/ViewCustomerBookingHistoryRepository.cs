using Application.Interfaces.ViewCustomerBookingHistory;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ViewCustomerBookingHistory
{
    public class ViewCustomerBookingHistoryRepository : RepositoryAsync<Domain.Entities.ViewBookingHistory.ViewCustomerBookingHistory, long>, IViewCustomerBookingHistoryRepository
    {
        public ViewCustomerBookingHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
