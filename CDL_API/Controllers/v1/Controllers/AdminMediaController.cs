using CDL.Models.Api;
using Microsoft.AspNetCore.Hosting;
using CDL.Models.Binder;
using CDL.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CDL.Models.Helpers.Constants;
using ApiRoutes = CDL.Api.Helpers.Constants;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
    [Route(ApiRoutes.AdminCmsRoute + "/media")]
    public class AdminMediaController : ControllerBase
    {
        private readonly IWebHostEnvironment env;

        public AdminMediaController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ApiResponse<MediaUploadResponse>> Upload(IFormFile? file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ApiResponse<MediaUploadResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.BadRequest, "Arquivo vazio.", new Exception("No file"));

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                    return ApiResponse<MediaUploadResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.BadRequest, "Tipo de arquivo não permitido.", new Exception("Bad ext"));

                var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
                var uploads = Path.Combine(webRoot, "uploads");
                Directory.CreateDirectory(uploads);

                var name = $"{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploads, name);
                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(stream);
                }

                var url = $"/uploads/{name}";
                return ApiResponse<MediaUploadResponse>.GetSuccessResponse(data: new MediaUploadResponse { Url = url });
            }
            catch (Exception ex)
            {
                return ApiResponse<MediaUploadResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
