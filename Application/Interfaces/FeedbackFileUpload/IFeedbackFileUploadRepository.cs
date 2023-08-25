using Application.Interfaces.Repositories;

namespace Application.Interfaces.FeedbackFileUpload
{
    public interface IFeedbackFileUploadRepository : IRepositoryAsync<Domain.Entities.FeedbackFileUpload.FeedbackFileUpload,long>
    {
    }
}
