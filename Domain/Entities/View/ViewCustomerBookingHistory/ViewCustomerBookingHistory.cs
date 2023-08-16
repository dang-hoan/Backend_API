using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.View.ViewCustomerBookingHistory
{

    [Keyless]
    public class ViewCustomerBookingHistory : AuditableBaseEntity<long>
    {
        [Column("customer_id", TypeName = "bigint")]
        public long CustomerId { get; set; }
        [Column("booking_id", TypeName = "bigint")]
        public long BookingId { get; set; }
        [Column("booking_date", TypeName = "datetime")]
        public DateTime BookingDate { get; set; }
        [Column("from_time", TypeName = "datetime")]
        public DateTime FromTime { get; set; }
        [Column("to_time", TypeName = "datetime")]
        public DateTime ToTime { get; set; }
        [Column("status", TypeName = "int")]
        public int? Status { get; set; }
        [Column("service_id", TypeName = "bigint")]
        public long ServiceId { get; set; }
        [Required]
        [Column("service_name", TypeName = "nvarchar(100)")]
        public string ServiceName { get; set; }
        [Required]
        [Column("price", TypeName = "decimal")]
        public decimal Price { get; set; }

    }
}
