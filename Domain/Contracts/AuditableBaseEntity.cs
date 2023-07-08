namespace Domain.Contracts
{
    public class AuditableBaseEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; } = default!;
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}