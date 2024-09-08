using locket.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace locket.Contexts
{
    public class LocketDbContext(DbContextOptions<LocketDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e=>e.Id).HasColumnName("id");
                entity.Property(e=>e.Username).HasColumnName("username");
                entity.Property(e=>e.Password).HasColumnName("password");
                entity.Property(e=>e.GoogleID).HasColumnName("google_id");
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.GoogleID).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
