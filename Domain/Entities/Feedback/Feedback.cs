using Domain.Constants.Enum;
using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Feedback
{
    [Table("feedback")]
    public class Feedback : AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_id", TypeName = "bigInt")]
        public long CustomerId { get; set; }

        [Required]
        [Column("service_id", TypeName = "bigInt")]
        public long ServiceId { get; set; }

        [Column("title", TypeName = "nvarchar(max)")]
        public string? Title { get; set; }

        [Column("staff_content", TypeName = "nvarchar(max)")]
        public string? StaffContent { get; set; }

        [Column("service_content", TypeName = "nvarchar(max)")]
        public string? ServiceContent { get; set; }

        [Column("reply_id", TypeName = "bigInt")]
        public long? ReplyId { get; set; }

        [Column("rating", TypeName = "smallInt")]
        public Rating? Rating { get; set; }

        [Column("booking_detail_id", TypeName = "bigInt")]
        public long BookingDetailId { get; set; }
    }
}