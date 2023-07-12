using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser<string>, IAuditableEntity<string>
    {
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; }
        public short TypeFlag { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}