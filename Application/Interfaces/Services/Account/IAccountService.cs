using Application.Dtos.Requests.Account;
using Domain.Wrappers;

namespace Application.Interfaces.Services.Account
{
    public interface IAccountService
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);
    }
}