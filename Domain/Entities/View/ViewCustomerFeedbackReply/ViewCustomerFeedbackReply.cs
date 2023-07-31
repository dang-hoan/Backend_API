using Domain.Constants.Enum;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.View.ViewCustomerFeedbackReply
{
    [Keyless]
    public class ViewCustomerFeedbackReply : AuditableBaseEntity<long>
    {
        [Column("feedback_id", TypeName = "bigint")]
        public long FeedbackId { get; set; }
        [Column("customer_id", TypeName = "bigint")]
        public long CustomerId { get; set; }
        [Column("customer_name", TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; }
        [Column("phone_number", TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }
        [Column("service_id", TypeName = "bigint")]
        public long ServiceId { get; set; }
        [Column("service_name", TypeName = "nvarchar(100)")]
        public string ServiceName { get; set; }
        [Column("feedback_title", TypeName = "nvarchar")]
        public string? FeedbackTitle { get; set; }
        [Column("feedback_service_content", TypeName = "nvarchar")]
        public string? FeedbackServiceContent { get; set; }
        [Column("feedback_staff_content", TypeName = "nvarchar")]
        public string? FeedbackStaffContent { get; set; }
        [Column("reply_id", TypeName = "bigint")]
        public long? ReplyId { get; set; }
        [Column("reply_title", TypeName = "nvarchar")]
        public string? ReplyTitle { get; set; }
        [Column("reply_content", TypeName = "nvarchar")]
        public string? ReplyContent { get; set; }
        [Column("rating", TypeName = "smallint")]
        public Rating? Rating { get; set; }
    }
}
