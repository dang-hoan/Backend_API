using Application.Dtos.Responses.FeedbackFileUpload;
using Application.Dtos.Responses.ServiceImage;
using Domain.Constants.Enum;

namespace Application.Features.Feedback.Queries.GetHistoryFeedback
{
    public class GetFeebackHistoryResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public List<ServiceImageResponse> ServiceImages { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long FeebackId { get; set; }
        public string? FeedbackTitle { get; set; }
        public string? FeedbackServiceContent { get; set; }
        public string? FeedbackStaffContent { get; set; }
        public Rating? Rating { get; set; }
        public ReplyResponse? Reply { get; set; }
        public DateTime CreatedOnFeedback { get; set; }
        public List<FeedbackFileUploadResponse> FeedbackFileUploads { get; set; }
    }

    public class ReplyResponse
    {
        public long Id { get; set; }
        public string? ReplyTitle { get; set; }
        public string? ReplyContent { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}