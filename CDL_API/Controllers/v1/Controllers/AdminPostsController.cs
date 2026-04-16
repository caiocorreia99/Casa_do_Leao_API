using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
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
    [Route(ApiRoutes.AdminCmsRoute + "/posts")]
    public class AdminPostsController : ControllerBase
    {
        private readonly ICmsAdminService cmsAdminService;

        public AdminPostsController(ICmsAdminService cmsAdminService)
        {
            this.cmsAdminService = cmsAdminService;
        }

        [HttpGet]
        public async Task<ApiResponse<PaggedList<PostListItemResponse>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? categoryId = null)
        {
            try
            {
                var data = await cmsAdminService.ListPostsAdminAsync(page, pageSize, categoryId);
                return ApiResponse<PaggedList<PostListItemResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaggedList<PostListItemResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResponse<PostDetailPublicResponse>> Get([FromRoute] int id)
        {
            try
            {
                var data = await cmsAdminService.GetPostAdminAsync(id);
                if (data == null)
                    return ApiResponse<PostDetailPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.NotFound, "Post não encontrado.", new Exception("Not found"));
                return ApiResponse<PostDetailPublicResponse>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PostDetailPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<Response>> Save([FromBody] PostAdminRequest request)
        {
            try
            {
                await cmsAdminService.SavePostAsync(request);
                return ApiResponse<Response>.GetSuccessResponse(message: "Salvo.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ApiResponse<Response>> Delete([FromRoute] int id)
        {
            try
            {
                await cmsAdminService.DeletePostAsync(id);
                return ApiResponse<Response>.GetSuccessResponse(message: "Removido.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
