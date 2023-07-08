using Application.Dtos.Requests;

namespace Application.Interfaces
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}