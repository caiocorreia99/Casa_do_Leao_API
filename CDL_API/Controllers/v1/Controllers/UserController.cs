using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ApiRoutes = CDL.Api.Helpers.Constants;
using static CDL.Models.Helpers.Constants;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    [Route(ApiRoutes.UserRoute)]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService UserService)
        {
            userService = UserService;
        }

        private int? ParseCallerUserId()
        {
            var v = User.FindFirst("uid")?.Value;
            return int.TryParse(v, out var id) ? id : null;
        }

        [Authorize]
        [HttpGet]
        [Route(ApiRoutes.UserRoute + "/get-user")]
        public async Task<ApiResponse<List<UserResponse>>> Get([FromQuery] int idUser)
        {
            try
            {
                var callerId = ParseCallerUserId();
                if (callerId == null)
                    return ApiResponse<List<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, HttpStatusCode.Unauthorized, "Não autenticado.", new Exception("No uid"));

                if (!User.IsInRole(UserRoles.Admin) && callerId != idUser)
                    return ApiResponse<List<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, HttpStatusCode.Forbidden, "Operação não permitida.", new Exception("Forbidden"));

                var result = await userService.GetUser(idUser);
                return ApiResponse<List<UserResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex); ;

            }
        }
        
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route(ApiRoutes.UserRoute + "/create-user")]
        public async Task<ApiResponse<Response>> CreateUser([FromBody] UserRequest userRequest)
        {
            try
            {
                await userService.CreateUser(userRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Usuário criado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(ApiRoutes.UserRoute + "/create-public-user")]
        public async Task<ApiResponse<Response>> CreatePublicUser([FromBody] UserRequest userRequest)
        {
            try
            {
                await userService.CreatePublicUser(userRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Usuário criado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [Authorize]
        [HttpPut]
        [Route(ApiRoutes.UserRoute + "/update-user")]
        public async Task<ApiResponse<Response>> UpdateUser([FromBody] UserRequest userRequest)
        {
            try
            {
                var callerId = ParseCallerUserId() ?? throw new Exception("Não autenticado.");
                var isAdmin = User.IsInRole(UserRoles.Admin);
                await userService.UpdateUser(userRequest, callerId, isAdmin);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Usuario Atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route(ApiRoutes.UserRoute + "/list")]
        public async Task<ApiResponse<PaggedList<UserResponse>>> ListUsers([FromQuery] int page = 1, int pageSize = 0, string? search = null)
        {
            try
            {
                var result = await userService.ListUsers(page, pageSize, search);
                return ApiResponse<PaggedList<UserResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                return ApiResponse<PaggedList<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete]
        [Route(ApiRoutes.UserRoute + "/disable-user")]
        public async Task<ApiResponse<Response>> DisableUser([FromQuery] int idUser)
        {
            try
            {
                await userService.DisableUser(idUser);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Usuário desativado com Sucesso.");
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }
    }
}
