using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace CDL.Api.Controllers.v1.Services
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IDatabaseFactory<DatabaseConnection> databaseFactory;

        public EventRegistrationService(IDatabaseFactory<DatabaseConnection> databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public async Task RegisterAsync(int userId, int eventId)
        {
            using var db = databaseFactory.Create();
            var ev = await db.Event.FirstOrDefaultAsync(e => e.IdEvent == eventId && e.Active && e.Published)
                ?? throw new Exception("Evento não encontrado ou indisponível.");

            var exists = await db.EventRegistration.AnyAsync(r => r.IdEvent == eventId && r.IdUser == userId && r.Active);
            if (exists)
                throw new Exception("Já inscrito neste evento.");

            await db.EventRegistration.AddAsync(new EventRegistration
            {
                IdEvent = eventId,
                IdUser = userId,
                RegisteredAt = DateTime.UtcNow,
                Active = true
            });
            await db.SaveChangesAsync();
        }

        public async Task CancelMyRegistrationAsync(int userId, int eventId)
        {
            using var db = databaseFactory.Create();
            var r = await db.EventRegistration
                .FirstOrDefaultAsync(x => x.IdEvent == eventId && x.IdUser == userId && x.Active)
                ?? throw new Exception("Inscrição não encontrada.");
            r.Active = false;
            await db.SaveChangesAsync();
        }

        public async Task<PaggedList<EventRegistrationRowResponse>> ListRegistrationsByEventAsync(int eventId, int page, int pageSize)
        {
            using var db = databaseFactory.Create();
            var query = db.EventRegistration
                .Include(r => r.User)
                .Where(r => r.IdEvent == eventId && r.Active);

            var totalCount = await query.CountAsync();
            if (pageSize <= 0) pageSize = Math.Max(totalCount, 1);
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);

            var rows = await query
                .OrderBy(r => r.RegisteredAt)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(r => new EventRegistrationRowResponse
                {
                    IdEventRegistration = r.IdEventRegistration,
                    IdUser = r.IdUser,
                    UserName = r.User.Name,
                    UserEmail = r.User.Email,
                    RegisteredAt = r.RegisteredAt
                })
                .ToListAsync();

            return new PaggedList<EventRegistrationRowResponse>(page, pageSize, pageRange, totalCount, rows);
        }
    }
}
