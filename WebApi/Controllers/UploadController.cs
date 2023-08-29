using Application.Dtos.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/v{version:apiVersion}/upload")]
    public class UploadController : BaseApiController<UploadController>
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Upload function common
        /// </summary>
        /// <param name="uploadRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(30 * 1024 * 1024)] //50MB Max upload request
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] UploadRequest uploadRequest)
        {
            var result = await _uploadService.UploadAsync(uploadRequest);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete Image
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteFile([FromForm] string filePath)
        {
            var result = await _uploadService.DeleteAsync(filePath);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}