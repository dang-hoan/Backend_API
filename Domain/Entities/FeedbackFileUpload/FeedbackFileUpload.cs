using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Contracts;

namespace Domain.Entities.FeedbackFileUpload
{
    [Table("feedback_fileupload")]
    public class FeedbackFileUpload : AuditableBaseEntity<long>
    {
        [Required]
        [Column("feedback_id", TypeName = "bigint")]
        public long FeedbackId { get; set; }

        [Required]
        [Column("name_file", TypeName = "varchar(MAX)")]
        public string NameFile { get; set; }

        [Required]
        [Column("type_file", TypeName = "varchar(MAX)")]
        public string TypeFile { get; set; }
    }
}