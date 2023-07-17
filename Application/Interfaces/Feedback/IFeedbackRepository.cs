using Application.Interfaces.Repositories;

namespace Application.Interfaces.Feedback
{
    public interface IFeedbackRepository : IRepositoryAsync<Domain.Entities.Feedback.Feedback, long>
    {
        
    }
}