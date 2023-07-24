using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Requests
{
    public class CheckImageSizeRequest
    {
        public List<IFormFile> Files { get; set; }
    }
}