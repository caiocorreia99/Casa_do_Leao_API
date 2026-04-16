using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IContentService
    {
        Task<List<AgendaItemPublicResponse>> GetAgendaPublicAsync();
        Task<PaggedList<PostListItemResponse>> GetPostsPublicAsync(int page, int pageSize, int? categoryId);
        Task<PostDetailPublicResponse?> GetPostPublicAsync(int idPost);
    }
}
