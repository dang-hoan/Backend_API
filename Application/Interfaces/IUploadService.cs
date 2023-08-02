using Application.Dtos.Requests;
using Application.Dtos.Responses.Upload;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IUploadService
    {

        Task<Result<UploadResponse>> UploadAsync(UploadRequest request);
        string GetFullUrl(string? filePath);
        Task<Result<bool>> DeleteAsync(string filePath);
    }
}