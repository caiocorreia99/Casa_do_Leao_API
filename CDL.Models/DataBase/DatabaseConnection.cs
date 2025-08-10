using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
        public virtual DbSet<UserFields> UserFields { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.IdUser);
                entity.HasOne(u => u.Fields)
                       .WithOne(f => f.User)
                       .HasForeignKey<UserFields>(f => f.IdUser)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserFields>(entity =>
            {
                entity.HasKey(f => f.IdUserFields);
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
