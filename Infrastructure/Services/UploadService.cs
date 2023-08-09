using Application.Dtos.Requests;
using Application.Dtos.Responses.Upload;
using Application.Exceptions;
using Application.Interfaces;
using Application.Shared;
using Domain.Wrappers;

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
    }
}