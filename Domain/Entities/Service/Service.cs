using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Service
{
    [Table("service")]
    public class Service : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name", TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column("service_time", TypeName = "int")]
        public int ServiceTime { get; set; }

        [Required]
        [Column("price", TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column("description", TypeName = "nvarchar(500)")]
        public string? Description { get; set; }

    }
}
