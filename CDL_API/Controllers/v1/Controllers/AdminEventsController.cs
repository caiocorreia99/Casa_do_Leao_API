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
    [Route(ApiRoutes.AdminCmsRoute + "/events")]
    public class AdminEventsController : ControllerBase
    {
        private readonly IEventService eventService;
        private readonly IEventRegistrationService registrationService;

        public AdminEventsController(IEventService eventService, IEventRegistrationService registrationService)
        {
            this.eventService = eventService;
            this.registrationService = registrationService;
        }

        [HttpGet]
        [Route("list")]
        public async Task<ApiResponse<PaggedList<EventResponse>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 0, [FromQuery] string? search = null)
        {
            try
            {
                var result = await eventService.ListEvents(page, pageSize, search);
                return ApiResponse<PaggedList<EventResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaggedList<EventResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet]
        [Route("get-event")]
        public async Task<ApiResponse<EventResponse>> GetEvent([FromQuery] int idEvent)
        {
            try
            {
                var result = await eventService.GetEvent(idEvent);
                return ApiResponse<EventResponse>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<EventResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpPost]
        [Route("create-event")]
        public async Task<ApiResponse<Response>> CreateEvent([FromBody] EventRequest eventRequest)
        {
            try
            {
                await eventService.CreateEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: "Evento criado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex);
            }
        }

        [HttpPut]
        [Route("update-event")]
        public async Task<ApiResponse<Response>> UpdateEvent([FromBody] EventRequest eventRequest)
        {
            try
            {
                await eventService.UpdateEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: "Evento Atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex);
            }
        }

        [HttpDelete]
        [Route("disable-event")]
        public async Task<ApiResponse<Response>> DisableEvent([FromQuery] EventRequest eventRequest)
        {
            try
            {
                await eventService.DisableEvent(eventRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: "Evento Desativado com Sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex);
            }
        }

        [Authorize(Policy = AuthPolicies.AdminOnly)]
        [HttpGet("{eventId:int}/registrations")]
        public async Task<ApiResponse<PaggedList<EventRegistrationRowResponse>>> ListRegistrations([FromRoute] int eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var data = await registrationService.ListRegistrationsByEventAsync(eventId, page, pageSize);
                return ApiResponse<PaggedList<EventRegistrationRowResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaggedList<EventRegistrationRowResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
