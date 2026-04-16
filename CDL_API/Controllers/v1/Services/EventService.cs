using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace CDL.Api.Controllers.v1.Services
{
    public class EventService : IEventService
    {
        private readonly IDatabaseFactory<DatabaseConnection> databaseFactory;

        public EventService(IDatabaseFactory<DatabaseConnection> databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public async Task<PaggedList<EventResponse>> ListEvents(int page, int pageSize, string? search = null)
        {
            using var db = databaseFactory.Create();

            IQueryable<Event> query = db.Event.Where(e => e.Active);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Title.Contains(search));

            var totalCount = await query.CountAsync();
            if (pageSize == 0) pageSize = totalCount;
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);

            var result = await query
                .OrderByDescending(e => e.StartDate)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToListAsync();

            var response = result.Select(e => new EventResponse
            {
                IdEvent = e.IdEvent,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                ImageUrl = e.ImageUrl,
                Published = e.Published
            }).ToList();

            return new PaggedList<EventResponse>(page, pageSize, pageRange, totalCount, response);
        }

        public async Task<PaggedList<EventResponse>> PublicListEvents(int page, int pageSize, string? search = null)
        {
            using var db = databaseFactory.Create();

            IQueryable<Event> query = db.Event.Where(e => e.Active && e.Published);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Title.Contains(search));

            var totalCount = await query.CountAsync();
            if (pageSize == 0) pageSize = totalCount;
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);

            var result = await query
                .OrderByDescending(e => e.StartDate)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToListAsync();

            var response = result.Select(e => new EventResponse
            {
                IdEvent = e.IdEvent,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                ImageUrl = e.ImageUrl,
                Published = e.Published
            }).ToList();

            return new PaggedList<EventResponse>(page, pageSize, pageRange, totalCount, response);
        }

        public async Task<EventResponse> GetPublicEvent(int idEvent)
        {
            using var db = databaseFactory.Create();

            var e = await db.Event.FirstOrDefaultAsync(x => x.IdEvent == idEvent && x.Active && x.Published);
            if (e == null)
                throw new Exception("Event not found");

            return new EventResponse
            {
                IdEvent = e.IdEvent,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                ImageUrl = e.ImageUrl,
                Published = e.Published
            };
        }

        public async Task<EventResponse> GetEvent(int idEvent)
        {
            using var db = databaseFactory.Create();

            var e = await db.Event.FirstOrDefaultAsync(x => x.IdEvent == idEvent && x.Active);
            if (e == null)
                throw new Exception("Event not found");

            return new EventResponse
            {
                IdEvent = e.IdEvent,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                ImageUrl = e.ImageUrl,
                Published = e.Published
            };
        }

        public async Task CreateEvent(EventRequest request)
        {
            using var db = databaseFactory.Create();

            var newEvent = new Event
            {
                Title = request.Title,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Location = request.Location,
                ImageUrl = request.ImageUrl,
                Published = request.Published,
                Active = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await db.Event.AddAsync(newEvent);
            await db.SaveChangesAsync();
        }

        public async Task UpdateEvent(EventRequest request)
        {
            using var db = databaseFactory.Create();

            var e = await db.Event.FirstOrDefaultAsync(x => x.IdEvent == request.IdEvent);
            if (e == null) 
                throw new Exception("Event not found");

            e.Title = request.Title ?? e.Title;
            e.Description = request.Description ?? e.Description;
            e.StartDate = request.StartDate;
            e.EndDate = request.EndDate;
            e.Location = request.Location ?? e.Location;
            e.ImageUrl = request.ImageUrl ?? e.ImageUrl;
            e.Published = request.Published;
            e.Active = request.Active;
            e.UpdatedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task DisableEvent(EventRequest eventResquest)
        {
            using var db = databaseFactory.Create();

            var e = await db.Event.FirstOrDefaultAsync(x => x.IdEvent == eventResquest.IdEvent);
            if (e == null) throw new Exception("Event not found");

            e.Active = false;
            e.UpdatedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }
    }

}
