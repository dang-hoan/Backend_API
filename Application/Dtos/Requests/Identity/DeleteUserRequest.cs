using Domain.Constants.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Requests.Identity
{
    public class DeleteUserRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public TypeFlagEnum TypeFlag { get; set; }
    }
}