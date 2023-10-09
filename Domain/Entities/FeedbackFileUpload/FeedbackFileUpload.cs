using Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.FeedbackFileUpload
{
    [Table("feedback_file_upload")]
    public class FeedbackFileUpload : AuditableBaseEntity<long>
    {
        [Required]
        [Column("feedback_id", TypeName = "bigint")]
        public long FeedbackId { get; set; }

        [Required]
        [Column("name_file", TypeName = "nvarchar(500)")]
        public string NameFile { get; set; }

        [Required]
        [Column("type_file", TypeName = "nvarchar(50)")]
        public string TypeFile { get; set; }
    }
}
