using CDL.Models.DataBase;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface ITokenService
    {
        string CreateJwtToken(User user, DateTime? expiration = null);
    }
}
