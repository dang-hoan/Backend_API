using Domain.Constants.Enum;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.View.ViewCustomerReviewHistory
{
    [Keyless]
    public class ViewCustomerReviewHistory : AuditableBaseEntity<long>
    {
        [Column("booking_id",TypeName = "bigInt")]
        public long BookingId { get; set; }
        [Column("service_id",TypeName = "bigInt")]
        public long ServiceId { get; set; }
        [Column("service_name",TypeName = "nvarchar(100)")]
        public string ServiceName { get; set; }
        [Column("customer_id",TypeName = "bigInt")]
        public long CustomerId { get; set; }
        [Column("customer_name",TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; }
        [Column("feedback_id",TypeName = "bigInt")]
        public long FeedbackId { get; set; }
        [Column("feedback_title",TypeName = "nvarchar")]
        public string? FeedbackTitle { get; set; }
        [Column("feedback_service_content",TypeName = "nvarchar")]
        public string? FeedbackServiceContent { get; set; }
        [Column("feedback_staff_content",TypeName = "nvarchar")]
        public string? FeedbackStaffContent { get; set; }
        [Column("reply_id",TypeName = "bigInt")]
        public long? ReplyId { get; set; }
        [Column("create_on_feedback",TypeName = "datetime")]
        public DateTime CreateOnFeedback { get; set; }
        [Column("rating", TypeName = "smallInt")]
        public Rating? Rating { get; set; }
    }
}
