using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Contracts;

namespace Domain.Entities.EnumMasterData
{
    [Table("enum_master_data")]
    public class EnumMasterData : AuditableBaseEntity<int>
    {
        [Required]
        [Column("value", TypeName = "nvarchar(50)")]
        public string Value { get; set; }

        [Required]
        [Column("enum_type", TypeName = "varchar(50)")]
        public string EnumType { get; set; }

    }
}
