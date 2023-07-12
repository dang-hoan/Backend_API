using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.BookingDetail
{
    [Table("booking_detail")]
    public class BookingDetail : AuditableBaseEntity<long>
    {
        [Required]
        [Column("booking_id", TypeName = "bigInt")]
        public long BookingId { get; set; }

        [Required]
        [Column("service_id", TypeName = "bigInt")]
        public long ServiceId { get; set; }

        [Column("note", TypeName = "nvarchar(500)")]
        public string? Note { get; set; }

    }
}
