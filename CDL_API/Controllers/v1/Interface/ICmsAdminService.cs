using CDL.Models.Api;
using CDL.Models.Binder;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface ICmsAdminService
    {
        Task<List<AgendaItemPublicResponse>> ListAgendaItemsAsync();
        Task<AgendaItemPublicResponse?> GetAgendaItemAsync(int id);
        Task SaveAgendaItemAsync(AgendaItemAdminRequest request);
        Task DeleteAgendaItemAsync(int id);

        Task<List<CategoryResponse>> ListCategoriesAsync();
        Task SaveCategoryAsync(CategoryAdminRequest request);
        Task DeleteCategoryAsync(int id);

        Task<PaggedList<PostListItemResponse>> ListPostsAdminAsync(int page, int pageSize, int? categoryId);
        Task<PostDetailPublicResponse?> GetPostAdminAsync(int idPost);
        Task SavePostAsync(PostAdminRequest request);
        Task DeletePostAsync(int idPost);
    }
}
