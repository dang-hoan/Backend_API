using Domain.Constants.Enum;

namespace Application.Dtos.Requests.Identity
{
    public class EditUserRequest
    {
        public long Id { get; set; }
        public TypeFlagEnum TypeFlag { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? ImageFile { get; set; }
    }
}
