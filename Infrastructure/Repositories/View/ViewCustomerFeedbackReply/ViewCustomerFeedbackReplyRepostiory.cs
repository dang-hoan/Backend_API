using Application.Interfaces.View.ViewCustomerFeedbackReply;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.View.ViewCustomerFeedbackReply
{
    public class ViewCustomerFeedbackReplyRepostiory : RepositoryAsync<Domain.Entities.View.ViewCustomerFeedbackReply.ViewCustomerFeedbackReply, long>, IViewCustomerFeedbackReplyRepository
    {
        public ViewCustomerFeedbackReplyRepostiory(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}