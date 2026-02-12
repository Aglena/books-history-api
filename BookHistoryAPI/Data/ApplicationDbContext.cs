using BookHistoryApi.Entities;
using BookHistoryApi.NewFolder;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookHistoryEntry> BookChangeHistories => Set<BookHistoryEntry>();
        public DbSet<Author> Authors => Set<Author>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);
        }
    }
}
