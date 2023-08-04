using Application.Interfaces.Feedback;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Feedback
{
    public class FeedbackRepository : RepositoryAsync<Domain.Entities.Feedback.Feedback, long>, IFeedbackRepository
    {
        public FeedbackRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}