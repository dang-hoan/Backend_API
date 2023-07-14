using Application.Interfaces;
using Application.Dtos.Requests;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class CheckFileType : ICheckFileType
    {
        public string CheckFilesIsImage(CheckImagesTypeRequest request)
        {
            foreach (IFormFile file in request.Files)
            {
                if (file != null)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".png", ".gif", ".jpeg" };
                    if (!allowedExtensions.Contains(extension))
                    {
                        return $"File {file.FileName} has invalid file extension! (Only extensions: {String.Join(", ", allowedExtensions)} are allowed)";
                    }

                    string[] allowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                    {
                        return "Invalid file type. Only image files are allowed.";
                    }
                }
            }
            return "";
        }
    }
}