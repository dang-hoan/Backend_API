using Infrastructure.Contexts;

namespace WebApi.Attributes
{
    public interface ICustomAuthorizeAttribute
    {
        Task OnCustomAuthorizationAsync(ApplicationDbContext context, string tokenString, string url, string action);
    }
}