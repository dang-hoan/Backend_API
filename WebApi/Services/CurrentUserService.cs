using System.Security.Claims;
using Application.Interfaces;

namespace WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
            RoleName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)!;
            HostServerName = httpContextAccessor.HttpContext?.Request.Scheme + "://" + httpContextAccessor.HttpContext?.Request.Host;
            OriginRequest = httpContextAccessor.HttpContext?.Request.Headers["Origin"].ToString()!;
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList()!;
        }
        public string UserId { get; }
        public string RoleName { get; }
        public string HostServerName { get; }
        public string OriginRequest { get; }
        public List<KeyValuePair<string, string>>? Claims { get; set; }
    }
}