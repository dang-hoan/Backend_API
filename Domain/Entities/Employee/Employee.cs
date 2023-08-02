using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Employee
{
    [Table("employee")]
    public class Employee : AuditableBaseEntity<long>
    {
        [Required]
        [Column("name", TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Column("gender", TypeName = "bit")]
        public bool? Gender { get; set; }

        [Column("birthday", TypeName = "datetime")]
        public DateTime? Birthday { get; set; }

        [Required]
        [Column("phone_number", TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }

        [Column("address", TypeName = "nvarchar(500)")]
        public string? Address { get; set; }

        [Required]
        [Column("email", TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Column("image", TypeName = "nvarchar(MAX)")]
        public string? Image { get; set; }

        [Required]
        [Column("work_shift_id", TypeName = "bigint")]
        public long WorkShiftId { get; set; }

    }
}
