using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Dtos.Requests.Identity;
using Application.Dtos.Responses.Identity;
using Application.Interfaces.Services.Identity;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;

namespace Infrastructure.Services.Identity
{
    public class IdentityService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppConfiguration _appConfig;

        public IdentityService(UserManager<AppUser> userManager, IOptions<AppConfiguration> appConfig)
        {
            _userManager = userManager;
            _appConfig = appConfig.Value;
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.EmployeeNo);
            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync("Incorrect username or password.");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync("Incorrect username or password.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                AvatarUrl = user.AvatarUrl!,
                Email = user.Email,
                EmployeeNo = user.UserName,
                Role = roles.First(),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                UserId = user.UserId
            };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return await Result<TokenResponse>.FailAsync("Invalid Token");
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userUsername = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userUsername);
            if (user == null)
                return await Result<TokenResponse>.FailAsync("Incorrect username or password.");
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailAsync("Invalid Token");
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                AvatarUrl = user.AvatarUrl!,
                Email = user.Email,
                EmployeeNo = user.UserName,
                Role = roles.First(),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        #region Private function

        private async Task<string> GenerateJwtAsync(AppUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserName),
                    (user.Email != null) ? new(ClaimTypes.Email, user.Email) : null,
                    new(ClaimTypes.Name, user.FullName),
                    new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                }
                .Union(userClaims)
                .Union(roleClaims);

            return claims;
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        #endregion Private function
    }
}