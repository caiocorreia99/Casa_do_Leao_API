using Microsoft.EntityFrameworkCore;

namespace CDL.Models.DataBase
{
    public partial class DatabaseConnection : DbContext
    {

        public DatabaseConnection()
        {
        }

        public DatabaseConnection(DbContextOptions<DatabaseConnection> options)
            : base(options)
        {
        }


        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserFields> UserFields { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventRegistration> EventRegistration { get; set; }
        public virtual DbSet<AgendaItem> AgendaItem { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<PostCarouselImage> PostCarouselImage { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.IdRole);
                entity.HasIndex(r => r.Code).IsUnique();
                entity.Property(r => r.Name).HasMaxLength(64).IsRequired();
                entity.Property(r => r.Code).HasMaxLength(32).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.IdUser);
                entity.HasOne(u => u.Fields)
                       .WithOne(f => f.User)
                       .HasForeignKey<UserFields>(f => f.IdUser)
                       .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.IdRole)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserFields>(entity =>
            {
                entity.HasKey(f => f.IdUserFields);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(f => f.IdEvent);
            });

            modelBuilder.Entity<EventRegistration>(entity =>
            {
                entity.HasKey(r => r.IdEventRegistration);
                entity.HasIndex(r => new { r.IdEvent, r.IdUser }).IsUnique();
                entity.HasOne(r => r.Event)
                    .WithMany(e => e.Registrations)
                    .HasForeignKey(r => r.IdEvent)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(r => r.User)
                    .WithMany(u => u.EventRegistrations)
                    .HasForeignKey(r => r.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AgendaItem>(entity =>
            {
                entity.HasKey(a => a.IdAgendaItem);
                entity.HasIndex(a => new { a.DayOfWeek, a.SortOrder });
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.IdCategory);
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.IdPost);
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(p => p.IdCategory)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostCarouselImage>(entity =>
            {
                entity.HasKey(x => x.IdPostCarouselImage);
                entity.HasOne(x => x.Post)
                    .WithMany(p => p.CarouselImages)
                    .HasForeignKey(x => x.IdPost)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(x => new { x.IdPost, x.SortOrder });
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
