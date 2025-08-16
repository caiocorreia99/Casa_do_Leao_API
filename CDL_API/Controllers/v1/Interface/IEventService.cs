using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IEventService
    {
        Task<EventResponse> GetEvent(int idEvent);
        Task CreateEvent(EventRequest eventRequest);
        Task<PaggedList<EventResponse>> ListEvents(int page, int pageSize, string? search = null);
        Task UpdateEvent(EventRequest eventRequest);
        Task DisableEvent(EventRequest eventRequest);
    }    
}
