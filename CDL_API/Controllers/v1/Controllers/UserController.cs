using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CDL.Models.Helpers.Constants;

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

        [AllowAnonymous]
        [HttpPost]
        [Route(Constants.UserRoute + "/create-user")]
        public async Task<ApiResponse<Response>> Post([FromBody] UserRequest userRequest)
        {
            try
            {
                await userService.CreateUser(userRequest);
                return ApiResponse<Response>.GetSuccessResponse(message: $"New user created.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_Generic, System.Net.HttpStatusCode.InternalServerError, ex.Message, exception: ex); ;
            }
        }
    }
}
