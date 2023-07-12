using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.Reply
{
    [Table("reply")]
    public class Reply : AuditableBaseEntity<long>
    {
        [Required]
        [Column("feedback_id", TypeName = "bigInt")]
        public long FeedbackId { get; set; }

        [Column("title", TypeName = "nvarchar(max)")]
        public string? Title { get; set; }

        [Column("content", TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

    }
}
