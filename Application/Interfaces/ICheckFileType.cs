using Application.Dtos.Requests;

namespace Application.Interfaces
{
    public interface ICheckFileType
    {
        string CheckFilesIsImage(CheckImagesTypeRequest request);
        string CheckFilesIsVideo(CheckVideoTypeRequest request);
    }
}