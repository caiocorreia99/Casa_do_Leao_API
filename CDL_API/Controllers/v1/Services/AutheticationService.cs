using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Controllers.v1.Models;
using CDL.Api.Helpers;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CDL.Api.Controllers.v1.Services
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly IDatabaseFactory<DatabaseConnection> database;
        private readonly ITokenService tokenService;
        private readonly APIEnvironment env;


        public AuthenticationService(
            IDatabaseFactory<DatabaseConnection> database,
            ITokenService tokenService,
            IOptions<APIEnvironment> env)
        {
            this.database = database;
            this.tokenService = tokenService;
            this.env = env.Value;
        }


        private void refreshUserToken(DatabaseConnection db, User user)
        {
            try
            {
                user.Token = tokenService.CreateJwtToken(user);
                user.LastLogin = DateTime.UtcNow;
                db.User.Update(user);
            }
            catch (Exception)
            {
                throw new Exception("Error on refresh token");
            }
        }

        public async Task<LoginResponse> Login(LoginRequest login)
        {

            using var db = this.database.Create();

            string passEncrypted = APIHelper.EncryptAES(login.Password, env.CypherPass);

            var authUser = await db.User.FirstOrDefaultAsync(u => u.Email == login.UserName && u.Password == passEncrypted)
                ?? throw new Exception("Invalid credentials");

            if (!authUser.Active)
                throw new Exception("User not authorized");

            refreshUserToken(db, authUser);

            await db.SaveChangesAsync();

            return new LoginResponse
            {
                IdUser = authUser.IdUser,
                Name = authUser.Name,
                Email = authUser.Email,
                Active = authUser.Active,
                Admin = authUser.Admin,
                Token = authUser.Token,                
            };
        }

        public async Task<LoginResponse> RefreshToken(int idUser)
        {

            using var db = this.database.Create();

            var user = await db.User.FirstOrDefaultAsync(x => x.IdUser == idUser)
                ?? throw new Exception("User not found");

            refreshUserToken(db, user);

            await db.SaveChangesAsync();

            return new LoginResponse()
            {
                IdUser = user.IdUser,
                Name = user.Name,
                Email = user.Email,
                Active = user.Active,
                Admin = user.Admin,
                Token = user.Token
            };

        }

        public async Task<bool> Logout(int IdUser)
        {

            using var db = this.database.Create();

            var user = await db.User.FirstOrDefaultAsync(x => x.IdUser == IdUser);

            if (user == null)
                throw new Exception("User not founded");

            user.Token = null;
            db.User.Update(user);
            await db.SaveChangesAsync();

            return true;

        }

    }
}
