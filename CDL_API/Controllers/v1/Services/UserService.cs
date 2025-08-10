using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Controllers.v1.Models;
using CDL.Api.Helpers;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CDL.Api.Controllers.v1.Services
{
    public class UserService : IUserService
    {

        private readonly IDatabaseFactory<DatabaseConnection> databaseFactory;
        private readonly APIEnvironment env;

        public UserService(
            IDatabaseFactory<DatabaseConnection> databaseFactory,
            IOptions<APIEnvironment> env)
        {
            this.databaseFactory = databaseFactory;
            this.env = env.Value;
        }

        public async Task<List<UserResponse>> GetUser(int idUser)
        {
            using var db = databaseFactory.Create();

            var result = await db.User.OrderBy(s => s.IdUser).Where(m => m.IdUser == idUser).ToListAsync();

            var userResponse = new List<UserResponse>();

            result.ForEach(user =>
            {
                userResponse.Add(new UserResponse
                {
                    IdUser = user.IdUser,
                    Name = user.Name,
                    Email = user.Email,
                    Admin = user.Admin,
                    Active = user.Active,
                });
            });

            return userResponse;
        }

        public async Task CreateUser(UserRequest userRequest)
        {

            using var db = databaseFactory.Create();

            if (userRequest == null)
                throw new Exception("object not sent");

            if (userRequest.Name == null)
                throw new Exception("Name not sent");

            if (userRequest.Email == null)
                throw new Exception("Email not sent");

            if (!APIHelper.ValidMail(userRequest.Email))
                throw new Exception("Invalid Email");

            var request = await db.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email);
            if (request != null) throw new Exception($"Email already in use");


            string passEncrypted = APIHelper.EncryptAES(userRequest.Password, env.CypherPass);

            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                Password = passEncrypted,
                Admin = userRequest.Admin,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Fields = new UserFields
                {
                    CPF = userRequest.CPF,
                    DataNascimento = userRequest.DataNascimento,
                    Telefone = userRequest.Telefone,
                    Endereco = userRequest.Endereco,
                    CEP = userRequest.CEP,
                    Cidade = userRequest.Cidade,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await db.User.AddAsync(user);
            await db.SaveChangesAsync();

        }
    }
}
