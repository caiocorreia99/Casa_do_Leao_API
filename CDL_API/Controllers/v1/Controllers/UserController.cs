using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CDL.Models.Helpers.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    [Route(Constants.UserRoute)]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService UserService)
        {
            userService = UserService;
        }

        [Authorize]
        [HttpGet]
        [Route(Constants.UserRoute + "/get-user")]
        public async Task<ApiResponse<List<UserResponse>>> Get([FromQuery] int idUser)
        {
            try
            {
                var result = await userService.GetUser(idUser);
                return ApiResponse<List<UserResponse>>.GetSuccessResponse(data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex); ;

            }
        }
        
        [HttpPost]
        [Route(Constants.UserRoute + "/create-user")]
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
        [Route(Constants.UserRoute + "/create-public-user")]
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
        [Route(Constants.UserRoute + "/update-user")]
        public async Task<ApiResponse<Response>> UpdateUser([FromBody] UserRequest userRequest)
        {
            try
            {
                await userService.UpdateUser(userRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"Usuario Atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }

        [Authorize]
        [HttpGet]
        [Route(Constants.UserRoute + "/list")]
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
                //await logService.SaveLog("CommandController", "ListCommand", urlRequest, ex.Message);
                return ApiResponse<PaggedList<UserResponse>>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route(Constants.UserRoute + "/disable-user")]
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
                //await logService.SaveLog("UserController", "Post", urlRequest, ex.Message);
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }
    }
}
