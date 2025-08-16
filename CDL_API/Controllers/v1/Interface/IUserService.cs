using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUser(int idUser);
        Task CreateUser(UserRequest userRequest);
        Task<PaggedList<UserResponse>> ListUsers(int page, int pageSize, string? search = null);
        Task UpdateUser(UserRequest userRequest);
        Task DisableUser(UserRequest userRequest);
    }
}
