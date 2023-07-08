using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}