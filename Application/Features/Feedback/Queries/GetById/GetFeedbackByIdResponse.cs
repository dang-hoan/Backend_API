using Domain.Constants.Enum;

namespace Application.Features.Feedback.Queries.GetById
{
    public class GetFeedbackByIdResponse
    {
        public long FeedbackId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? FeedbackTitle { get; set; }
        public string? FeedbackServiceContent { get; set; }
        public string? FeedbackStaffContent { get; set; }
        public long? ReplyId { get; set; }
        public string? ReplyTitle { get; set; }
        public string? ReplyContent { get; set; }
        public Rating? Rating { get; set; }
    }
}