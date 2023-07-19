using Domain.Constants.Enum;

namespace Application.Features.Feedback.Queries.GetAll
{
    public class GetAllFeedbackResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string ServiceName { get; set; }
        public Rating? Rating { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}