using Application.Dtos.Requests;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class RemoveImageService : IRemoveImageService
    {
        public bool RemoveImage(RemoveImageRequest request)
        {
            var relativePath = request.FilePath;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (!File.Exists(relativePath))
                return true;

            try
            {
                File.Delete(fullPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}