using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Constants.Enum;
using Domain.Contracts;

namespace Domain.Entities.Booking
{
    [Table("booking")]
    public class Booking : AuditableBaseEntity<long>
    {
        [Column("customer_id",TypeName = "bigint")]
        public long CustomerId { get; set; }

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
