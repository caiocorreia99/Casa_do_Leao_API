using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUser(int idUser);
        Task CreateUser(UserRequest userRequest);
        Task CreatePublicUser(UserRequest userRequest);        
        Task<PaggedList<UserResponse>> ListUsers(int page, int pageSize, string? search = null);
        Task UpdateUser(UserRequest userRequest, int callerUserId, bool callerIsAdmin);
        Task DisableUser(int idUser);
    }
}
