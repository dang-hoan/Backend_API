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
        private string _userName;
        public string Username
        {
            get
            {
                _userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                return _userName;
            }
        }

        private string _roleName;
        public string RoleName
        {
            get
            {
                _roleName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "";
                return _roleName;
            }
        }

        public List<KeyValuePair<string, string>>? Claims { get; set; }
    }
}