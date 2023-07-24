using Application.Dtos.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class CheckFileSize : ICheckSizeFile
    {
        public string CheckImageSize(CheckImageSizeRequest request)
        {
            string result = "";
            foreach (IFormFile file in request.Files)
            {
                long fileSize = file.Length;
                long maxSizeImage = ICheckSizeFile.IMAGE_MAX_SIZE; //5MB  
                if (fileSize >= maxSizeImage)
                {
                    result = $"Do not upload image over max size {maxSizeImage / (1024 * 1024)} MB";
                    return result;
                }

            }
            return result;
        }

        public string CheckVideoSize(CheckVideoSizeRequest request)
        {
            string result = "";
            foreach (IFormFile file in request.Files)
            {
                long fileSize = file.Length;
                long maxSizeVideo = ICheckSizeFile.VIDEO_MAX_SIZE; //30MB

                if (fileSize >= maxSizeVideo)
                {
                    result = $"Do not upload video over max size {maxSizeVideo / (1024 * 1024)} MB";
                    return result;
                }

            }
            return result;
        }
    }
}