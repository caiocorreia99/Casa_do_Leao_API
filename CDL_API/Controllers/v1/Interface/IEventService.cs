using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IEventService
    {
        Task<EventResponse> GetEvent(int idEvent);
        Task CreateEvent(EventRequest eventRequest);
        Task<PaggedList<EventResponse>> ListEvents(int page, int pageSize, string? search = null);
        Task<PaggedList<EventResponse>> PublicListEvents(int page, int pageSize, string? search = null);
        Task<EventResponse> GetPublicEvent(int idEvent);
        Task UpdateEvent(EventRequest eventRequest);
        Task DisableEvent(EventRequest eventRequest);
    }    
}
