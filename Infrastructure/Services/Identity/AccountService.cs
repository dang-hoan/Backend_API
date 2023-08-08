using Application.Dtos.Requests.Account;
using Application.Interfaces.Services.Account;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string employeeNo)
        {
            var user = await this._userManager.FindByNameAsync(employeeNo);
            if (user == null)
            {
                return await Result.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                return await Result.FailAsync("Password confirmation does not match.");
            }

            var identityResult = await this._userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(identityResult.ToString());
        }
        public async Task<bool> IsExistUsername(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null;
        }
        public async Task<bool> AddAcount(AppUser user, string password,string role)
        {
            await _userManager.CreateAsync(user, password);
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
    }
}