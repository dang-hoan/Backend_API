using Application.Dtos.Requests.Identity;
using Application.Dtos.Requests.SendEmail;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Azure.Core;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using IResult = Domain.Wrappers.IResult;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _mailService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<AppUser> userManager, IEmailService mailService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _mailService = mailService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                throw new ApiException(StaticVariable.NOT_FOUND_MSG, "Email Not Found.");
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //send mail
            var route = "auth/reset-password";
            var endpointUri = new Uri(string.Concat($"{request.UrlFE}/", route));
            //var filePath = _environment.WebRootPath + "\\templates\\forgot-password-sender.html";
            //var str = new StreamReader(filePath);
            //var mailText = await str.ReadToEndAsync();
            //str.Close();
            var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

            //var logo = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}/logo/logo-nineplus.png";
            //var mailBody = mailText.Replace("[EndPointUrl]", HtmlEncoder.Default.Encode(passwordResetUrl)).Replace("[Logo]", logo);
            var mailRequest = new EmailRequest()
            {
                Body = passwordResetUrl,
                Subject = "Reset Password",
                To = request.Email
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(); 
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request.Password.Length < 8) return await Result.FailAsync(StaticVariable.INVALID_PASSWORD);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync("System Error");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync("System Error");
        }

        public async Task<IResult> DeleteUser(DeleteUserRequest request)
        {
            var user = _userManager.Users.Where(user => user.UserId == request.Id && user.TypeFlag == request.TypeFlag).FirstOrDefault();
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync("System Error");
            }

            user.IsActive = false;
            user.IsDeleted = true;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync("System Error");
        }

        public async Task<IResult> EditUser(EditUserRequest request)
        {
            var user = _userManager.Users.Where(user => user.UserId == request.Id && user.TypeFlag == request.TypeFlag).FirstOrDefault();
            if (user == null)
            {
                return await Result.FailAsync("This user does not exist in the database");
            }
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;
            user.AvatarUrl = request.ImageFile;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync();
            }
            return await Result.FailAsync("System Error");
        }
    }
}