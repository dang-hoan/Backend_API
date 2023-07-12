using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.Customer
{
    [Table("customer")]
    public class Customer : AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_name", TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; }

        [Required]
        [Column("phone_number", TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }

        [Column("address", TypeName = "nvarchar(500)")]
        public string? Address { get; set; }

        [Column("date_of_birth", TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }

        [Column("total_money", TypeName = "decimal")]
        public decimal? TotalMoney { get; set; }

    }
}
