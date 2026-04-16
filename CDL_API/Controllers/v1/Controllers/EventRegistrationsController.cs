using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CDL.Models.Helpers.Constants;
using ApiRoutes = CDL.Api.Helpers.Constants;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    [Authorize]
    [Route(ApiRoutes.EventsApiRoute)]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly IEventRegistrationService registrationService;

        public EventRegistrationsController(IEventRegistrationService registrationService)
        {
            this.registrationService = registrationService;
        }

        private int ParseUserId()
        {
            var v = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(v) || !int.TryParse(v, out var id))
                throw new Exception("Token inválido.");
            return id;
        }

        [HttpPost("{eventId:int}/registrations")]
        public async Task<ApiResponse<Response>> Register([FromRoute] int eventId)
        {
            try
            {
                var userId = ParseUserId();
                await registrationService.RegisterAsync(userId, eventId);
                return ApiResponse<Response>.GetSuccessResponse(message: "Inscrição realizada.");
            }
            catch (Exception ex)
            {
                var status = ex.Message.Contains("Já inscrito", StringComparison.OrdinalIgnoreCase)
                    ? HttpStatusCode.Conflict
                    : HttpStatusCode.InternalServerError;
                if (status == HttpStatusCode.Conflict)
                    return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, status, ex.Message, ex);
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, status, ex.Message, ex);
            }
        }

        [HttpDelete("{eventId:int}/registrations/me")]
        public async Task<ApiResponse<Response>> Cancel([FromRoute] int eventId)
        {
            try
            {
                var userId = ParseUserId();
                await registrationService.CancelMyRegistrationAsync(userId, eventId);
                return ApiResponse<Response>.GetSuccessResponse(message: "Inscrição cancelada.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
    }
}
