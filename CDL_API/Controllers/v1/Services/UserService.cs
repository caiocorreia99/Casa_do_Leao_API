using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Controllers.v1.Models;
using CDL.Api.Helpers;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using CDL.Models.Helpers;
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

        private static async Task<int> GetRoleIdByCodeAsync(DatabaseConnection db, string roleCode)
        {
            var code = UserRoles.Resolve(roleCode, false);
            var id = await db.Role.AsNoTracking()
                .Where(r => r.Code == code)
                .Select(r => r.IdRole)
                .FirstOrDefaultAsync();
            if (id != 0) return id;
            return await db.Role.AsNoTracking()
                .Where(r => r.Code == UserRoles.Simple)
                .Select(r => r.IdRole)
                .FirstAsync();
        }

        public async Task<PaggedList<UserResponse>> ListUsers(int page, int pageSize, string? search = null)
        {
            using var db = databaseFactory.Create();

            IQueryable<User> query = db.User
                .Include(u => u.Fields)
                .Include(u => u.Role)
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

            var usersResponse = result.Select(user => ToUserResponse(user)).ToList();

            return new PaggedList<UserResponse>(page, pageSize, pageRange, totalCount, usersResponse);
        }

        public async Task UpdateUser(UserRequest userRequest, int callerUserId, bool callerIsAdmin)
        {
            using var db = databaseFactory.Create();

            if (userRequest.IdUser == null)
                throw new Exception("IdUser não informado.");

            var user = await db.User
                .Include(u => u.Fields)
                .FirstOrDefaultAsync(u => u.IdUser == userRequest.IdUser);

            if (user == null)
                throw new Exception("Usuario nao encontrado");

            if (!callerIsAdmin && user.IdUser != callerUserId)
                throw new Exception("Operação não permitida para este usuário.");

            if (userRequest.Email != null && !APIHelper.ValidMail(userRequest.Email))
                throw new Exception("Email com formato invalido");

            if (!string.IsNullOrEmpty(userRequest.Email))
            {
                var existing = await db.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email && u.IdUser != user.IdUser);
                if (existing != null)
                    throw new Exception("Email ja esta sendo usado por ouro usuário, inseria um e-mail diferente.");
                user.Email = userRequest.Email;
            }

            if (!string.IsNullOrEmpty(userRequest.Name))
                user.Name = userRequest.Name;

            if (callerIsAdmin)
            {
                user.Active = userRequest.Active;
                if (!string.IsNullOrWhiteSpace(userRequest.Role))
                {
                    var role = UserRoles.Resolve(userRequest.Role, false);
                    user.IdRole = await GetRoleIdByCodeAsync(db, role);
                    user.Admin = role == UserRoles.Admin;
                }
                else
                {
                    var currentCode = await db.Role.AsNoTracking()
                        .Where(r => r.IdRole == user.IdRole)
                        .Select(r => r.Code)
                        .FirstAsync();
                    user.Admin = userRequest.Admin;
                    var merged = UserRoles.Resolve(currentCode, user.Admin);
                    user.IdRole = await GetRoleIdByCodeAsync(db, merged);
                    user.Admin = merged == UserRoles.Admin;
                }
            }

            if (!string.IsNullOrEmpty(userRequest.Password))
                user.Password = APIHelper.EncryptAES(userRequest.Password, env.CypherPass);

            user.UpdatedAt = DateTime.Now;

            if (user.Fields != null)
            {
                user.Fields.CPF = userRequest.CPF ?? user.Fields.CPF;
                user.Fields.DataNascimento = userRequest.DataNascimento ?? user.Fields.DataNascimento;
                user.Fields.Telefone = userRequest.Telefone ?? user.Fields.Telefone;
                user.Fields.Endereco = userRequest.Endereco ?? user.Fields.Endereco;
                user.Fields.Numero = userRequest.Numero ?? user.Fields.Numero;
                user.Fields.CEP = userRequest.CEP ?? user.Fields.CEP;
                user.Fields.Cidade = userRequest.Cidade ?? user.Fields.Cidade;
                user.Fields.UpdatedAt = DateTime.Now;
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<UserResponse>> GetUser(int idUser)
        {
            using var db = databaseFactory.Create();

            var result = await db.User
                .Include(u => u.Role)
                .OrderBy(s => s.IdUser)
                .Where(m => m.IdUser == idUser)
                .ToListAsync();

            return result.Select(ToUserResponse).ToList();
        }

        private static UserResponse ToUserResponse(User user)
        {
            var role = user.Role?.Code;
            if (string.IsNullOrWhiteSpace(role) || !UserRoles.IsValid(role))
                role = user.Admin ? UserRoles.Admin : UserRoles.Simple;
            return new UserResponse
            {
                IdUser = user.IdUser,
                Name = user.Name,
                Email = user.Email,
                Admin = user.Admin,
                IdRole = user.IdRole,
                Role = role,
                Active = user.Active,
            };
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

            var role = !string.IsNullOrWhiteSpace(userRequest.Role)
                ? UserRoles.Resolve(userRequest.Role, false)
                : UserRoles.NormalizeFromLegacy(userRequest.Admin);

            var idRole = await GetRoleIdByCodeAsync(db, role);

            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                Password = passEncrypted,
                Admin = role == UserRoles.Admin,
                IdRole = idRole,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Fields = new UserFields
                {
                    CPF = userRequest.CPF,
                    DataNascimento = userRequest.DataNascimento,
                    Telefone = userRequest.Telefone,
                    Endereco = userRequest.Endereco,
                    Numero = userRequest.Numero ?? string.Empty,
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

            var idUsuario = await GetRoleIdByCodeAsync(db, UserRoles.Simple);

            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                Password = passEncrypted,
                Admin = false,
                IdRole = idUsuario,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Fields = new UserFields
                {
                    CPF = userRequest.CPF,
                    DataNascimento = userRequest.DataNascimento,
                    Telefone = userRequest.Telefone,
                    Endereco = userRequest.Endereco,
                    Numero = userRequest.Numero ?? string.Empty,
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
