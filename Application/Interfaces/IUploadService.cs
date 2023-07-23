﻿using Application.Dtos.Requests;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
        string GetImageLink(string relativePath, IHttpContextAccessor httpContextAccessor);
    }
}