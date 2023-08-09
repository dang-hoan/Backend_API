using Application.Dtos.Requests;
using Application.Dtos.Responses.Upload;
using Application.Exceptions;
using Application.Interfaces;
using Application.Shared;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        private readonly ICurrentUserService _currentUserService;

        public UploadService(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<Result<UploadResponse>> UploadAsync(UploadRequest request)
        {
            var fileName = $"{DateTime.Now:yyyyMMddHHmmsss}_{request.File.FileName}";
            var folderName = Path.Combine("Files", request.FilePath);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var isImage = CheckFilesIsImage(request);
            var isVideo = CheckFilesIsVideo(request);
            var isMaxSizeImage = CheckImageSize(request);
            var isMaxSizeVideo = CheckVideoSize(request);
            if (!isImage && !isVideo)
            {
                return await Result<UploadResponse>.FailAsync($"{request.File.FileName} is not an valid image or video");
            }
            if (!isMaxSizeImage && isImage)
            {
                return await Result<UploadResponse>.FailAsync($"{request.File.FileName} is over allowed image size(5MB) ");
            }
            if (!isMaxSizeVideo && isVideo)
            {
                return await Result<UploadResponse>.FailAsync($"{request.File.FileName} is over allowed video size(30MB)");
            }

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var dbPath = Path.Combine(folderName, fileName);

            using (var stream = new FileStream(dbPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var result = new UploadResponse()
            {
                FilePath = dbPath,
                FileUrl = Path.Combine(_currentUserService.HostServerName, dbPath).Replace("\\", "/")
            };

            return await Result<UploadResponse>.SuccessAsync(result);
        }

        public string GetFullUrl(string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var result = _currentUserService.HostServerName + "/" + filePath;
                return result;
            }

            return "";
        }

        public Task<Result<bool>> DeleteAsync(string filePath)
        {
            var fileToDelete = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            if (File.Exists(fileToDelete))
            {
                File.Delete(fileToDelete);
            }
            return Result<bool>.SuccessAsync(true, ApplicationConstants.SuccessMessage.DeletedSuccess); ;
            //throw new ApiException(ApplicationConstants.ErrorMessage.NotFound);
        }

        public static bool CheckFilesIsImage(UploadRequest request)
        {
            if (request.File != null)
            {
                string extension = Path.GetExtension(request.File.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".png", ".gif", ".jpeg" };
                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }

                string[] allowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };
                if (!allowedMimeTypes.Contains(request.File.ContentType.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckFilesIsVideo(UploadRequest request)
        {
            if (request.File != null)
            {
                string extension = Path.GetExtension(request.File.FileName).ToLower();
                string[] allowedImagesExtensions = { ".mp3", ".mp4", ".mpeg" };
                if (!allowedImagesExtensions.Contains(extension))
                {
                    return false;
                }

                string[] allowedMimeTypes = { "video/mp3", "video/mp4", "video/mpeg" };
                if (!allowedMimeTypes.Contains(request.File.ContentType.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckImageSize(UploadRequest request)
        {
            long fileSize = request.File.Length;
            long maxSizeImage = ICheckSizeFile.IMAGE_MAX_SIZE; //5MB  
            if (fileSize >= maxSizeImage)
            {
                return false;
            }

            return true;
        }

        public static bool CheckVideoSize(UploadRequest request)
        {
            long fileSize = request.File.Length;
            long maxSizeVideo = ICheckSizeFile.VIDEO_MAX_SIZE; //30MB

            if (fileSize >= maxSizeVideo)
            {
                return false;
            }

            return true;
        }
    }
}