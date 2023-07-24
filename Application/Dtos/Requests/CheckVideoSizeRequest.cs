using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Requests
{
    public class CheckVideoSizeRequest
    {
        public List<IFormFile> Files { get; set; }
        
    }
}