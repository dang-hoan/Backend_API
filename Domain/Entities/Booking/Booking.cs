using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Constants.Enum;
using Domain.Contracts;

namespace Domain.Entities.Booking
{
    [Table("booking")]
    public class Booking : AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_name", TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; }

        [Required]
        [Column("phone_number", TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column("booking_date", TypeName = "datetime")]
        public DateTime BookingDate { get; set; }

        [Required]
        [Column("from_time", TypeName = "datetime")]
        public DateTime FromTime { get; set; }

        [Required]
        [Column("to_time", TypeName = "datetime")]
        public DateTime Totime { get; set; }

        [Column("note", TypeName = "nvarchar(500)")]
        public string? Note { get; set; }

        [Column("status", TypeName = "smallInt")]
        public BookingStatus? Status { get; set; }

    }
}
