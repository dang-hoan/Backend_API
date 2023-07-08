using Application.Dtos.Requests.Identity;
using Application.Dtos.Responses.Identity;
using Domain.Wrappers;

namespace Application.Interfaces.Services.Identity
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}