using BookHistoryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookEvent> BookEvents => Set<BookEvent>();
        public DbSet<Author> Authors => Set<Author>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            builder.Entity<BookEvent>()
                .HasOne(e => e.Book)
                .WithMany(b => b.Events)
                .HasForeignKey(e => e.BookId);
        }
    }
}
