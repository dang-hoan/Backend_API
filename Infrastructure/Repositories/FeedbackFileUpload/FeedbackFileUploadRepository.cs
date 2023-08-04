using Application.Interfaces.FeedbackFileUpload;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.FeedbackFileUpload
{
    public class FeedbackFileUploadRepository : RepositoryAsync<Domain.Entities.FeedbackFileUpload.FeedbackFileUpload, long>, IFeedbackFileUploadRepository
    {
        public FeedbackFileUploadRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}