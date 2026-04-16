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
    [Route(ApiRoutes.AdminCmsRoute + "/categories")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICmsAdminService cmsAdminService;

        public AdminCategoriesController(ICmsAdminService cmsAdminService)
        {
            this.cmsAdminService = cmsAdminService;
        }

        [HttpGet]
        public async Task<ApiResponse<List<CategoryResponse>>> List()
        {
            try
            {
                var data = await cmsAdminService.ListCategoriesAsync();
                return ApiResponse<List<CategoryResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<Response>> Save([FromBody] CategoryAdminRequest request)
        {
            try
            {
                await cmsAdminService.SaveCategoryAsync(request);
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
                await cmsAdminService.DeleteCategoryAsync(id);
                return ApiResponse<Response>.GetSuccessResponse(message: "Removido.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
