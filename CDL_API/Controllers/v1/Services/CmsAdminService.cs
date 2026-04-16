using CDL.Api.Controllers.v1.Interface;
using CDL.Models.Api;
using CDL.Models.Binder;
using CDL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CDL.Api.Controllers.v1.Services
{
    public class CmsAdminService : ICmsAdminService
    {
        private readonly IDatabaseFactory<DatabaseConnection> databaseFactory;

        public CmsAdminService(IDatabaseFactory<DatabaseConnection> databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        private static string Slugify(string name, string? slug)
        {
            if (!string.IsNullOrWhiteSpace(slug))
                return slug.Trim().ToLowerInvariant();
            var s = Regex.Replace(name.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-");
            return s.Trim('-');
        }

        public async Task<List<AgendaItemPublicResponse>> ListAgendaItemsAsync()
        {
            using var db = databaseFactory.Create();
            return await db.AgendaItem
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
        }

        public async Task<AgendaItemPublicResponse?> GetAgendaItemAsync(int id)
        {
            using var db = databaseFactory.Create();
            return await db.AgendaItem
                .Where(a => a.IdAgendaItem == id)
                .Select(a => new AgendaItemPublicResponse
                {
                    IdAgendaItem = a.IdAgendaItem,
                    DayOfWeek = a.DayOfWeek,
                    Title = a.Title,
                    TimeDisplay = a.TimeDisplay,
                    SortOrder = a.SortOrder,
                    Active = a.Active
                })
                .FirstOrDefaultAsync();
        }

        public async Task SaveAgendaItemAsync(AgendaItemAdminRequest request)
        {
            using var db = databaseFactory.Create();
            if (request.IdAgendaItem.HasValue)
            {
                var a = await db.AgendaItem.FirstOrDefaultAsync(x => x.IdAgendaItem == request.IdAgendaItem.Value)
                    ?? throw new Exception("Agenda item não encontrado.");
                a.DayOfWeek = request.DayOfWeek;
                a.Title = request.Title;
                a.TimeDisplay = request.TimeDisplay;
                a.SortOrder = request.SortOrder;
                a.Active = request.Active;
                a.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await db.AgendaItem.AddAsync(new AgendaItem
                {
                    DayOfWeek = request.DayOfWeek,
                    Title = request.Title,
                    TimeDisplay = request.TimeDisplay,
                    SortOrder = request.SortOrder,
                    Active = request.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }

        public async Task DeleteAgendaItemAsync(int id)
        {
            using var db = databaseFactory.Create();
            var a = await db.AgendaItem.FirstOrDefaultAsync(x => x.IdAgendaItem == id)
                ?? throw new Exception("Agenda item não encontrado.");
            db.AgendaItem.Remove(a);
            await db.SaveChangesAsync();
        }

        public async Task<List<CategoryResponse>> ListCategoriesAsync()
        {
            using var db = databaseFactory.Create();
            return await db.Category.OrderBy(c => c.SortOrder).Select(c => new CategoryResponse
            {
                IdCategory = c.IdCategory,
                Name = c.Name,
                Slug = c.Slug,
                SortOrder = c.SortOrder
            }).ToListAsync();
        }

        public async Task SaveCategoryAsync(CategoryAdminRequest request)
        {
            using var db = databaseFactory.Create();
            var slug = Slugify(request.Name, request.Slug);

            if (request.IdCategory.HasValue)
            {
                var c = await db.Category.FirstOrDefaultAsync(x => x.IdCategory == request.IdCategory.Value)
                    ?? throw new Exception("Categoria não encontrada.");
                var dup = await db.Category.AnyAsync(x => x.Slug == slug && x.IdCategory != c.IdCategory);
                if (dup) throw new Exception("Slug já em uso.");
                c.Name = request.Name;
                c.Slug = slug;
                c.SortOrder = request.SortOrder;
            }
            else
            {
                if (await db.Category.AnyAsync(x => x.Slug == slug))
                    throw new Exception("Slug já em uso.");
                await db.Category.AddAsync(new Category
                {
                    Name = request.Name,
                    Slug = slug,
                    SortOrder = request.SortOrder
                });
            }
            await db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            using var db = databaseFactory.Create();
            var hasPosts = await db.Post.AnyAsync(p => p.IdCategory == id);
            if (hasPosts)
                throw new Exception("Categoria possui posts e não pode ser excluída.");
            var c = await db.Category.FirstOrDefaultAsync(x => x.IdCategory == id)
                ?? throw new Exception("Categoria não encontrada.");
            db.Category.Remove(c);
            await db.SaveChangesAsync();
        }

        public async Task<PaggedList<PostListItemResponse>> ListPostsAdminAsync(int page, int pageSize, int? categoryId)
        {
            using var db = databaseFactory.Create();
            var query = db.Post.Include(p => p.Category).Where(p => p.Active);
            if (categoryId.HasValue)
                query = query.Where(p => p.IdCategory == categoryId.Value);
            var totalCount = await query.CountAsync();
            if (pageSize <= 0) pageSize = Math.Max(totalCount, 1);
            var pageRange = (int)Math.Ceiling(totalCount / (decimal)pageSize);
            var list = await query
                .OrderByDescending(p => p.UpdatedAt)
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

        public async Task<PostDetailPublicResponse?> GetPostAdminAsync(int idPost)
        {
            using var db = databaseFactory.Create();
            var p = await db.Post
                .Include(x => x.Category)
                .Include(x => x.CarouselImages)
                .FirstOrDefaultAsync(x => x.IdPost == idPost && x.Active);
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

        public async Task SavePostAsync(PostAdminRequest request)
        {
            using var db = databaseFactory.Create();
            var catExists = await db.Category.AnyAsync(c => c.IdCategory == request.IdCategory);
            if (!catExists)
                throw new Exception("Categoria inválida.");

            Post post;
            if (request.IdPost.HasValue)
            {
                post = await db.Post
                    .Include(p => p.CarouselImages)
                    .FirstOrDefaultAsync(p => p.IdPost == request.IdPost.Value)
                    ?? throw new Exception("Post não encontrado.");
                post.IdCategory = request.IdCategory;
                post.Title = request.Title;
                post.BannerImageUrl = request.BannerImageUrl;
                post.HtmlBody = request.HtmlBody ?? "";
                var wasPublished = post.Published;
                post.Published = request.Published;
                if (request.Published && !wasPublished && post.PublishedAt == null)
                    post.PublishedAt = DateTime.UtcNow;
                if (!request.Published)
                    post.PublishedAt = null;
                post.AuthorUserId = request.AuthorUserId;
                post.Active = request.Active;
                post.UpdatedAt = DateTime.UtcNow;

                db.PostCarouselImage.RemoveRange(post.CarouselImages);
            }
            else
            {
                post = new Post
                {
                    IdCategory = request.IdCategory,
                    Title = request.Title,
                    BannerImageUrl = request.BannerImageUrl,
                    HtmlBody = request.HtmlBody ?? "",
                    Published = request.Published,
                    PublishedAt = request.Published ? DateTime.UtcNow : null,
                    AuthorUserId = request.AuthorUserId,
                    Active = request.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await db.Post.AddAsync(post);
                await db.SaveChangesAsync();
            }

            if (request.IdPost.HasValue)
                await db.SaveChangesAsync();

            var order = 0;
            foreach (var dto in request.CarouselImages.OrderBy(x => x.SortOrder))
            {
                await db.PostCarouselImage.AddAsync(new PostCarouselImage
                {
                    IdPost = post.IdPost,
                    ImageUrl = dto.ImageUrl,
                    SortOrder = dto.SortOrder != 0 ? dto.SortOrder : order++,
                    AltText = dto.AltText
                });
            }
            await db.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int idPost)
        {
            using var db = databaseFactory.Create();
            var p = await db.Post.FirstOrDefaultAsync(x => x.IdPost == idPost)
                ?? throw new Exception("Post não encontrado.");
            p.Active = false;
            p.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}
