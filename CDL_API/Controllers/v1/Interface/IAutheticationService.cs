using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest login);
        Task<LoginResponse> RefreshToken(int idUser);
        Task<bool> Logout(int IdUser);

    }
}
