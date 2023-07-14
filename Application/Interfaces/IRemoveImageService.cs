using Application.Dtos.Requests;

namespace Application.Interfaces
{
    public interface IRemoveImageService
    {
        bool RemoveImage(RemoveImageRequest request);
    }
}