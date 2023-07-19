﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;
using Domain.Constants.Enum;

namespace Domain.Entities.Feedback
{
    [Table("feedback")]
    public class Feedback : AuditableBaseEntity<long>
    {
        [Required]
        [Column("customer_id", TypeName = "bigInt")]
        public long CustomerId { get; set; }

        [Required]
        [Column("service_id", TypeName = "bigInt")]
        public long ServiceId { get; set; }

        [Column("title", TypeName = "nvarchar(max)")]
        public string? Title { get; set; }

        [Column("content", TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        [Column("reply_id", TypeName = "bigInt")]
        public long? ReplyId { get; set; }

        [Column("rating", TypeName = "smallInt")]
        public Rating? Rating { get; set; }

    }
}
