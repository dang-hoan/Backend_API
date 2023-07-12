using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        [Column("image", TypeName = "nvarchar(200)")]
        public string? Image { get; set; }


        [Required]
        [Column("user_name", TypeName = "nvarchar(50)")]
        public string Username { get; set; }


        [Required]
        [Column("password", TypeName = "nvarchar(100)")]
        public string Password { get; set; }


        [Required]
        [Column("work_shift_id", TypeName = "bigint")]
        public long WorkShiftId { get; set; }

    }
}
