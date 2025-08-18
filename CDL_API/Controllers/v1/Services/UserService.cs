using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Controllers.v1.Models;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<PaggedList<UserResponse>> ListUsers(int page, int pageSize, string? search = null)
        {
            using var db = databaseFactory.Create();

            IQueryable<User> query = db.User
                .Include(u => u.Fields) // se precisar dos dados extras
                .Where(m => m.Active != false);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Name.Contains(search));

            var totalCount = await query.CountAsync();
            if (pageSize == 0) pageSize = totalCount;
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);

            var result = await query
                .OrderBy(s => s.IdUser)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToListAsync();

            // 🔹 Mapeia User → UserResponse
            var usersResponse = result.Select(user => new UserResponse
            {
                IdUser = user.IdUser,
                Name = user.Name,
                Email = user.Email,
                Admin = user.Admin,
                Active = user.Active
            }).ToList();

            return new PaggedList<UserResponse>(page, pageSize, pageRange, totalCount, usersResponse);
        }

        public async Task UpdateUser(UserRequest userRequest)
        {
            using var db = databaseFactory.Create();

            var user = await db.User
                .Include(u => u.Fields)
                .FirstOrDefaultAsync(u => u.IdUser == userRequest.IdUser);

            if (user == null)
                throw new Exception("Usuario nao encontrado");

            if (userRequest.Email != null && !APIHelper.ValidMail(userRequest.Email))
                throw new Exception("Email com formato invalido");

            // verifica se email já está em uso por outro user
            if (!string.IsNullOrEmpty(userRequest.Email))
            {
                var existing = await db.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email && u.IdUser != userRequest.IdUser);
                if (existing != null)
                    throw new Exception("Email ja esta sendo usado por ouro usuário, inseria um e-mail diferente.");
                user.Email = userRequest.Email;
            }

            if (!string.IsNullOrEmpty(userRequest.Name))
                user.Name = userRequest.Name;

            user.Admin = userRequest.Admin;
            user.Active = userRequest.Active;
            user.UpdatedAt = DateTime.Now;

            // Atualiza os campos extras
            if (user.Fields != null)
            {
                user.Fields.CPF = userRequest.CPF ?? user.Fields.CPF;
                user.Fields.DataNascimento = userRequest.DataNascimento ?? user.Fields.DataNascimento;
                user.Fields.Telefone = userRequest.Telefone ?? user.Fields.Telefone;
                user.Fields.Endereco = userRequest.Endereco ?? user.Fields.Endereco;
                user.Fields.CEP = userRequest.CEP ?? user.Fields.CEP;
                user.Fields.Cidade = userRequest.Cidade ?? user.Fields.Cidade;
                user.Fields.UpdatedAt = DateTime.Now;
            }

            await db.SaveChangesAsync();
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
                throw new Exception("Objeto vazio");

            if (userRequest.Name == null)
                throw new Exception("O Nome nao foi inserido.");

            if (userRequest.Email == null)
                throw new Exception("Email nao foi inserido");

            if (!APIHelper.ValidMail(userRequest.Email))
                throw new Exception("Email com formato invalido");

            var request = await db.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email);
            if (request != null) throw new Exception($"Email ja esta sendo usado por ouro usuário, inseria um e-mail diferente.");


            string passEncrypted = APIHelper.EncryptAES(userRequest.Password, env.CypherPass);

            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                Password = passEncrypted,
                Admin = userRequest.Admin,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Fields = new UserFields
                {
                    CPF = userRequest.CPF,
                    DataNascimento = userRequest.DataNascimento,
                    Telefone = userRequest.Telefone,
                    Endereco = userRequest.Endereco,
                    CEP = userRequest.CEP,
                    Cidade = userRequest.Cidade,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            await db.User.AddAsync(user);
            await db.SaveChangesAsync();

        }

        public async Task CreatePublicUser(UserRequest userRequest)
        {
            using var db = databaseFactory.Create();

            if (userRequest == null)
                throw new Exception("Objeto vazio");

            if (userRequest.Name == null)
                throw new Exception("O Nome nao foi inserido.");

            if (userRequest.Email == null)
                throw new Exception("Email nao foi inserido");

            if (!APIHelper.ValidMail(userRequest.Email))
                throw new Exception("Email com formato invalido");

            var request = await db.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email);
            if (request != null) throw new Exception($"Email ja esta sendo usado por ouro usuário, inseria um e-mail diferente.");


            string passEncrypted = APIHelper.EncryptAES(userRequest.Password, env.CypherPass);

            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                Password = passEncrypted,
                Admin = false,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Fields = new UserFields
                {
                    CPF = userRequest.CPF,
                    DataNascimento = userRequest.DataNascimento,
                    Telefone = userRequest.Telefone,
                    Endereco = userRequest.Endereco,
                    CEP = userRequest.CEP,
                    Cidade = userRequest.Cidade,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            await db.User.AddAsync(user);
            await db.SaveChangesAsync();

        }

        public async Task DisableUser(int idUser)
        {

            using var db = databaseFactory.Create();

            var user = db.User.FirstOrDefault(s => s.IdUser == idUser) ??
                throw new Exception("Usuário não encontrado.");

            user.Active = false;

            db.User.Update(user);
            await db.SaveChangesAsync();

        }
    }
}
