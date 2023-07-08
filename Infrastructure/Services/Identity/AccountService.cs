using Application.Dtos.Requests.Account;
using Application.Interfaces.Services.Account;
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
                return await Result.FailAsync("Không tìm thấy người dùng");
            }

            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                return await Result.FailAsync("Mật khẩu không khớp.");
            }

            var identityResult = await this._userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync("Đổi mật khẩu không thành công");
        }
    }
}