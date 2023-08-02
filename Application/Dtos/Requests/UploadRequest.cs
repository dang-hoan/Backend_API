using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Requests
{
    public class UploadRequest
    {
        public IFormFile File { get; set; } = default!;
        public string FilePath { get; set; } = default!;
    }
}