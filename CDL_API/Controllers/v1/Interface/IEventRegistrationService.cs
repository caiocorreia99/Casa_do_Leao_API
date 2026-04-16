using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IEventRegistrationService
    {
        Task RegisterAsync(int userId, int eventId);
        Task CancelMyRegistrationAsync(int userId, int eventId);
        Task<PaggedList<EventRegistrationRowResponse>> ListRegistrationsByEventAsync(int eventId, int page, int pageSize);
    }
}
