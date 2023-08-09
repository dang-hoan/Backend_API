using Application.Dtos.Requests.Account;
using Application.Dtos.Requests.Identity;
using Application.Interfaces;
using Application.Interfaces.Services.Account;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Account
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public AccountController(IAccountService accountService, ICurrentUserService currentUserService, IUserService userService)
        {
            _accountService = accountService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePasswordAsync(request, _currentUserService.UserName);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var result = await _userService.ForgotPasswordAsync(request);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = await _userService.ResetPasswordAsync(request);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}