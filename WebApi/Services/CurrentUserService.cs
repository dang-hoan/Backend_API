using System.Security.Claims;
using Application.Interfaces;

namespace WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string RoleName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
        public string HostServerName => _httpContextAccessor.HttpContext?.Request.Scheme + "://" + _httpContextAccessor.HttpContext?.Request.Host;
        public string OriginRequest => _httpContextAccessor.HttpContext?.Request.Headers["Origin"].ToString();
        public List<KeyValuePair<string, string>> Claims =>
            _httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable()
                .Select(item => new KeyValuePair<string, string>(item.Type, item.Value))
                .ToList();
    }
}