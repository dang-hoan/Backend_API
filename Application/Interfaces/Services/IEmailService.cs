using Application.Dtos.Requests.SendEmail;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);

        Task SendMultiAsync(EmailMultiRequest request);
    }
}