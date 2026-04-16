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
    [Route(ApiRoutes.EventRoute)]
    public class EventController : ControllerBase
    {

        private readonly IEventService eventService;

        public EventController(IEventService EventService)
        {
            eventService = EventService;
        }

        [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
        [HttpGet]
        [Route(ApiRoutes.EventRoute + "/get-event")]
        public async Task<ApiResponse<EventResponse>> GetEvent([FromQuery] int idEvent)
        {
            try
            {
                var result = await eventService.GetEvent(idEvent);
                return ApiResponse<EventResponse>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<EventResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex); ;

            }
        }

        [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
        [HttpPost]
        [Route(ApiRoutes.EventRoute + "/create-event")]
        public async Task<ApiResponse<Response>> CreateEvent([FromBody] EventRequest eventRequest)
        {
            try
            {
                await eventService.CreateEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Evento criado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }
        
        [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
        [HttpPut]
        [Route(ApiRoutes.EventRoute + "/update-event")]
        public async Task<ApiResponse<Response>> UpdateEvent([FromBody] EventRequest eventRequest)
        {
            try
            {
                await eventService.UpdateEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Evento Atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
        [HttpGet]
        [Route(ApiRoutes.EventRoute + "/list")]
        public async Task<ApiResponse<PaggedList<EventResponse>>> ListEvents([FromQuery] int page = 1, int pageSize = 0, string? search = null)
        {
            try
            {
                var result = await eventService.ListEvents(page, pageSize, search);
                return ApiResponse<PaggedList<EventResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                //await logService.SaveLog("CommandController", "ListCommand", urlRequest, ex.Message);
                return ApiResponse<PaggedList<EventResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [Authorize(Policy = AuthPolicies.EditorOrAdmin)]
        [HttpDelete]
        [Route(ApiRoutes.EventRoute + "/disable-event")]
        public async Task<ApiResponse<Response>> DisableEvent([FromQuery] EventRequest eventRequest)
        {
            try
            {
                await eventService.DisableEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Evento Desativado com Sucesso.");
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                //await logService.SaveLog("UserController", "Post", urlRequest, ex.Message);
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(ApiRoutes.EventRoute + "/public-list")]
        public async Task<ApiResponse<PaggedList<EventResponse>>> PublicListEvents([FromQuery] int page = 1, int pageSize = 0, string? search = null)
        {
            try
            {
                var result = await eventService.PublicListEvents(page, pageSize, search);
                return ApiResponse<PaggedList<EventResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                //await logService.SaveLog("CommandController", "ListCommand", urlRequest, ex.Message);
                return ApiResponse<PaggedList<EventResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
