using Application.Dtos.Requests;

namespace Application.Interfaces
{
    public interface ICheckSizeFile
    {
        const long IMAGE_MAX_SIZE = 5 * 1024 * 1024;
        const long VIDEO_MAX_SIZE = 30 * 1024 * 1024;
        string CheckImageSize(CheckImageSizeRequest request);
        string CheckVideoSize(CheckVideoSizeRequest request);
    }
}