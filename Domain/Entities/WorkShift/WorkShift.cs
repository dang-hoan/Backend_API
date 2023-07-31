using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.WorkShift
{
    [Table("work_shift")]
    public class WorkShift : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name", TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column("working_from_time", TypeName = "time")]
        public TimeSpan WorkingFromTime { get; set; }

        [Required]
        [Column("working_to_time", TypeName = "time")]
        public TimeSpan WorkingToTime { get; set; }

        [Column("is_default", TypeName = "bit")]
        public bool? IsDefault { get; set; }

        [Column("description", TypeName = "nvarchar(500)")]
        public string? Description { get; set; }

        [Column("work_days", TypeName = "varchar(30)")]
        public string WorkDays { get; set; }
    }
}
