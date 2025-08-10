using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUser(int idUser);
        Task CreateUser(UserRequest userRequest);
    }
}
