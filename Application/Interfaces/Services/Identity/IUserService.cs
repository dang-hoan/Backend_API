using Application.Dtos.Requests.Identity;
using Domain.Wrappers;

namespace Application.Interfaces.Services.Identity
{
    public interface IUserService
    {
        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult> DeleteUser(DeleteUserRequest request);
        Task<IResult> EditUser(EditUserRequest request);
    }
}