using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ServiceImage
{
    [Table("service_image")]
    public class ServiceImage : AuditableBaseEntity<long>
    {
        [Required]
        [Column("service_id", TypeName = "bigint")]
        public long ServiceId { get; set; }

        [Required]
        [Column("name_file", TypeName = "nvarchar(MAX)")]
        public string NameFile { get; set; }

    }
}
