using CDL.Api.Controllers.v1.Helpers;
using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static CDL.Models.Helpers.Constants;

namespace CDL.Api.Controllers.v1.Controllers
{
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(Constants.AuthenticationRoute)]
        public async Task<ApiResponse<LoginResponse>> Login([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] LoginRequest login)
        {
            try
            {
                var loginResponse = await authService.Login(login);
                return ApiResponse<LoginResponse>.GetSuccessResponse(data: loginResponse);
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                return ApiResponse<LoginResponse>.GetErrorResponse(InternalCode.Catch_ValidateLogin, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route(Constants.AuthenticationRoute + "/refresh-token")]
        public async Task<ApiResponse<LoginResponse>> RefreshToken([FromQuery] int idUser)
        {
            try
            {
                var loginResponse = await authService.RefreshToken(idUser);
                return ApiResponse<LoginResponse>.GetSuccessResponse(data: loginResponse);
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                return ApiResponse<LoginResponse>.GetErrorResponse(InternalCode.Catch_ValidateLogin, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(Constants.AuthenticationRoute + "/logout/{IdUser}")]
        public async Task<ApiResponse<Response>> Logout([FromRoute] int IdUser)
        {
            try
            {
                if (await authService.Logout(IdUser))
                {
                    return ApiResponse<Response>.GetSuccessResponse(message: "Session close", data: new Response());
                }
                else
                {
                    throw new Exception("Logout error");
                }
            }
            catch (Exception ex)
            {
                var urlRequest = $"{Request.Host}{Request.Path}{Request.QueryString}";
                return ApiResponse<Response>.GetErrorResponse(InternalCode.Catch_ValidateLogin, System.Net.HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

    }
}
