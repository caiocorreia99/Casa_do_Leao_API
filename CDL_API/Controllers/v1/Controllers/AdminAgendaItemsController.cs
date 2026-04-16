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
    [Route(ApiRoutes.AdminCmsRoute + "/agenda-items")]
    public class AdminAgendaItemsController : ControllerBase
    {
        private readonly ICmsAdminService cmsAdminService;

        public AdminAgendaItemsController(ICmsAdminService cmsAdminService)
        {
            this.cmsAdminService = cmsAdminService;
        }

        [HttpGet]
        public async Task<ApiResponse<List<AgendaItemPublicResponse>>> List()
        {
            try
            {
                var data = await cmsAdminService.ListAgendaItemsAsync();
                return ApiResponse<List<AgendaItemPublicResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AgendaItemPublicResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResponse<AgendaItemPublicResponse>> Get([FromRoute] int id)
        {
            try
            {
                var data = await cmsAdminService.GetAgendaItemAsync(id);
                if (data == null)
                    return ApiResponse<AgendaItemPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.NotFound, "Item não encontrado.", new Exception("Not found"));
                return ApiResponse<AgendaItemPublicResponse>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<AgendaItemPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<Response>> Save([FromBody] AgendaItemAdminRequest request)
        {
            try
            {
                await cmsAdminService.SaveAgendaItemAsync(request);
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
                await cmsAdminService.DeleteAgendaItemAsync(id);
                return ApiResponse<Response>.GetSuccessResponse(message: "Removido.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
