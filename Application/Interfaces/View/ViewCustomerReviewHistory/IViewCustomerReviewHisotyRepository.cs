using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.View.ViewCustomerReviewHistory
{
    public interface IViewCustomerReviewHisotyRepository : IRepositoryAsync<Domain.Entities.View.ViewCustomerReviewHistory.ViewCustomerReviewHistory,long>
    {
    }
}
