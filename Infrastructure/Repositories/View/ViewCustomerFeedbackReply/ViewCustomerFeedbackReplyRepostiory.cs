using Application.Interfaces.View.ViewCustomerFeedbackReply;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.View.ViewCustomerFeedbackReply
{
    public class ViewCustomerFeedbackReplyRepostiory : RepositoryAsync<Domain.Entities.View.ViewCustomerFeedbackReply.ViewCustomerFeedbackReply, long>, IViewCustomerFeedbackReplyRepository
    {
        public ViewCustomerFeedbackReplyRepostiory(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
