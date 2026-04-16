using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CDL.Models.Helpers.Constants;
using ApiRoutes = CDL.Api.Helpers.Constants;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route(ApiRoutes.ContentRoute)]
    public class ContentController : ControllerBase
    {
        private readonly IContentService contentService;
        private readonly IEventService eventService;

        public ContentController(IContentService contentService, IEventService eventService)
        {
            this.contentService = contentService;
            this.eventService = eventService;
        }

        [HttpGet]
        [Route("agenda")]
        public async Task<ApiResponse<List<AgendaItemPublicResponse>>> GetAgenda()
        {
            try
            {
                var data = await contentService.GetAgendaPublicAsync();
                return ApiResponse<List<AgendaItemPublicResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AgendaItemPublicResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet]
        [Route("posts")]
        public async Task<ApiResponse<PaggedList<PostListItemResponse>>> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null)
        {
            try
            {
                var data = await contentService.GetPostsPublicAsync(page, pageSize, categoryId);
                return ApiResponse<PaggedList<PostListItemResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaggedList<PostListItemResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet]
        [Route("posts/{id:int}")]
        public async Task<ApiResponse<PostDetailPublicResponse>> GetPost([FromRoute] int id)
        {
            try
            {
                var data = await contentService.GetPostPublicAsync(id);
                if (data == null)
                    return ApiResponse<PostDetailPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.NotFound, "Post não encontrado.", new Exception("Not found"));
                return ApiResponse<PostDetailPublicResponse>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PostDetailPublicResponse>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet]
        [Route("events")]
        public async Task<ApiResponse<PaggedList<EventResponse>>> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            try
            {
                var data = await eventService.PublicListEvents(page, pageSize, search);
                return ApiResponse<PaggedList<EventResponse>>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaggedList<EventResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpGet]
        [Route("events/{id:int}")]
        public async Task<ApiResponse<EventResponse>> GetEvent([FromRoute] int id)
        {
            try
            {
                var data = await eventService.GetPublicEvent(id);
                return ApiResponse<EventResponse>.GetSuccessResponse(data: data);
            }
            catch (Exception ex)
            {
                var status = ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? System.Net.HttpStatusCode.NotFound
                    : System.Net.HttpStatusCode.InternalServerError;
                return ApiResponse<EventResponse>.GetErrorResponse(InternalCode.Catch_Generic, status, ex.Message, ex);
            }
        }
    }
}
