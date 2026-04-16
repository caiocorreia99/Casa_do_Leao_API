using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace CDL.Api.Controllers.v1.Services
{
    public class ContentService : IContentService
    {
        private readonly IDatabaseFactory<DatabaseConnection> databaseFactory;

        public ContentService(IDatabaseFactory<DatabaseConnection> databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public async Task<List<AgendaItemPublicResponse>> GetAgendaPublicAsync()
        {
            using var db = databaseFactory.Create();
            var items = await db.AgendaItem
                .Where(a => a.Active)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.SortOrder)
                .Select(a => new AgendaItemPublicResponse
                {
                    IdAgendaItem = a.IdAgendaItem,
                    DayOfWeek = a.DayOfWeek,
                    Title = a.Title,
                    TimeDisplay = a.TimeDisplay,
                    SortOrder = a.SortOrder,
                    Active = a.Active
                })
                .ToListAsync();
            return items;
        }

        public async Task<PaggedList<PostListItemResponse>> GetPostsPublicAsync(int page, int pageSize, int? categoryId)
        {
            using var db = databaseFactory.Create();
            var query = db.Post
                .Include(p => p.Category)
                .Where(p => p.Active && p.Published);

            if (categoryId.HasValue)
                query = query.Where(p => p.IdCategory == categoryId.Value);

            var totalCount = await query.CountAsync();
            if (pageSize <= 0) pageSize = Math.Max(totalCount, 1);
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);

            var list = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => new PostListItemResponse
                {
                    IdPost = p.IdPost,
                    Title = p.Title,
                    BannerImageUrl = p.BannerImageUrl,
                    IdCategory = p.IdCategory,
                    CategoryName = p.Category.Name,
                    PublishedAt = p.PublishedAt
                })
                .ToListAsync();

            return new PaggedList<PostListItemResponse>(page, pageSize, pageRange, totalCount, list);
        }

        public async Task<PostDetailPublicResponse?> GetPostPublicAsync(int idPost)
        {
            using var db = databaseFactory.Create();
            var p = await db.Post
                .Include(x => x.Category)
                .Include(x => x.CarouselImages)
                .FirstOrDefaultAsync(x => x.IdPost == idPost && x.Active && x.Published);

            if (p == null) return null;

            return new PostDetailPublicResponse
            {
                IdPost = p.IdPost,
                IdCategory = p.IdCategory,
                Title = p.Title,
                BannerImageUrl = p.BannerImageUrl,
                HtmlBody = p.HtmlBody,
                Published = p.Published,
                PublishedAt = p.PublishedAt,
                CategoryName = p.Category.Name,
                CarouselImages = p.CarouselImages
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new PostCarouselImageDto
                    {
                        IdPostCarouselImage = c.IdPostCarouselImage,
                        ImageUrl = c.ImageUrl,
                        SortOrder = c.SortOrder,
                        AltText = c.AltText
                    }).ToList()
            };
        }
    }
}
