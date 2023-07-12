using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.EmployeeService
{
    [Table("employee_service")]
    public class EmployeeService : AuditableBaseEntity<long>
    {
        [Required]
        [Column("employee_id", TypeName = "bigint")]
        public long EmployeeId { get; set; }

        [Required]
        [Column("service_id", TypeName = "bigint")]
        public long ServiceId { get; set; }

        [Column("note", TypeName = "nvarchar(500)")]
        public string? Note { get; set; }

    }
}
