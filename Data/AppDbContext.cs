using System;
using System.Threading;
using System.Threading.Tasks;
using GerenciadorDeLivraria.Domain;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeLivraria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var book = modelBuilder.Entity<Book>();

            book.ToTable("Books");
            book.HasKey(b => b.Id);

            book.HasIndex(b => new { b.Title, b.Author }).IsUnique();

            book.Property(b => b.Title).HasMaxLength(120).IsRequired();
            book.Property(b => b.Author).HasMaxLength(120).IsRequired();

            book.Property(b => b.Price).HasConversion<double>();
        }


        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Atualiza apenas o UpdatedAt
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
