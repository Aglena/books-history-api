using BookHistoryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            await db.Database.MigrateAsync();

            if (await db.Books.AnyAsync())
                return;

            var (books, events) = CreateDemoData();
            db.Books.AddRange(books);
            db.BookEvents.AddRange(events);

            await db.SaveChangesAsync();
        }


        private static (List<Book> Books, List<BookEvent> Events) CreateDemoData()
        {
            var now = DateTime.UtcNow;

            var alice = new Author { Name = "Alice Johnson" };
            var bob = new Author { Name = "Bob Smith" };
            var clara = new Author { Name = "Clara Nguyen" };
            var dan = new Author { Name = "Dan Novak" };
            var emma = new Author { Name = "Emma Rossi" };

            var books = new List<Book>
            {
                new() { Id = 1, Title = "Refactoring for Humans (2nd Ed.)", Description = "Practical refactoring patterns and habits.", PublishDate = new DateOnly(2025, 3, 18), Authors = new List<Author> { alice } },
                new() { Id = 2, Title = "Orbit of Paper Planets", Description = "Light sci-fi about origami-coded messages.", PublishDate = new DateOnly(2026, 1, 9), Authors = new List<Author> { bob } },
                new() { Id = 3, Title = "Gardens of Late Winter", Description = "Essays on winter landscapes and patience.", PublishDate = new DateOnly(2023, 12, 5), Authors = new List<Author> { clara } },
                new() { Id = 4, Title = "Five-Minute Dinners, One Pan", Description = "Fast weeknight recipes with minimal cleanup.", PublishDate = new DateOnly(2022, 6, 21), Authors = new List<Author> { emma } },
                new() { Id = 5, Title = "The Mistelbach Ledger", Description = "A novel about an archivist and a hidden ledger.", PublishDate = new DateOnly(2024, 9, 12), Authors = new List<Author> { dan } },
                new() { Id = 6, Title = "Clean APIs, Calm Minds", Description = "API design for maintainable systems.", PublishDate = new DateOnly(2024, 2, 2), Authors = new List<Author> { alice, bob } },
                new() { Id = 7, Title = "SQLite in Practice", Description = "Hands-on guide to SQLite for small services.", PublishDate = new DateOnly(2021, 10, 11), Authors = new List<Author> { clara } },
                new() { Id = 8, Title = "The Pragmatic Interview", Description = "How to present engineering trade-offs clearly.", PublishDate = new DateOnly(2025, 8, 20), Authors = new List<Author> { dan, emma } },
                new() { Id = 9, Title = "Tales from the Build Server", Description = "Short stories about CI, failures, and fixes.", PublishDate = new DateOnly(2020, 4, 7), Authors = new List<Author> { bob } },
                new() { Id = 10, Title = "Domain Language Matters", Description = "Why naming shapes architecture and code.", PublishDate = new DateOnly(2023, 5, 30), Authors = new List<Author> { alice, clara } }
            };

            var events = new List<BookEvent>
            {
                NewCreated(books[0], now.AddMinutes(-120), "Book created"),
                NewTitleUpdated(books[0], now.AddMinutes(-90), $"Title was changed to \"{books[0].Title}\""),

                NewCreated(books[1], now.AddMinutes(-110), "Book created"),
                NewUpdated(books[1], now.AddMinutes(-80), EventTarget.BookDescription, "Description was updated"),

                NewCreated(books[2], now.AddMinutes(-100), "Book created"),
                NewUpdated(books[2], now.AddMinutes(-70), EventTarget.BookPublishDate, $"Publish date was changed to \"{books[2].PublishDate}\""),

                NewCreated(books[3], now.AddMinutes(-95), "Book created"),
                NewAuthorAdded(books[3], now.AddMinutes(-60), "Author \"Alice Johnson\" was added"),

                NewCreated(books[4], now.AddMinutes(-90), "Book created"),
                NewAuthorRemoved(books[4], now.AddMinutes(-55), "Author \"Dan Novak\" was removed"),

                NewCreated(books[5], now.AddMinutes(-85), "Book created"),
                NewTitleUpdated(books[5], now.AddMinutes(-50), $"Title was changed to \"{books[5].Title}\""),

                NewCreated(books[6], now.AddMinutes(-80), "Book created"),
                NewUpdated(books[6], now.AddMinutes(-45), EventTarget.BookDescription, "Description was updated"),

                NewCreated(books[7], now.AddMinutes(-75), "Book created"),
                NewUpdated(books[7], now.AddMinutes(-40), EventTarget.BookPublishDate, $"Publish date was changed to \"{books[7].PublishDate}\""),

                NewCreated(books[8], now.AddMinutes(-70), "Book created"),
                NewTitleUpdated(books[8], now.AddMinutes(-35), $"Title was changed to \"{books[8].Title}\""),

                NewCreated(books[9], now.AddMinutes(-65), "Book created"),
                NewAuthorAdded(books[9], now.AddMinutes(-30), "Author \"Emma Rossi\" was added"),
            };

            return (books, events);
        }

        private static BookEvent NewCreated(Book book, DateTime atUtc, string description) =>
            new()
            {
                BookId = book.Id,
                OccuredAt = atUtc,
                Target = EventTarget.Book,
                Type = EventType.Created,
                Description = description
            };

        private static BookEvent NewUpdated(Book book, DateTime atUtc, EventTarget target, string description) =>
            new()
            {
                BookId = book.Id,
                OccuredAt = atUtc,
                Target = target,
                Type = EventType.Updated,
                Description = description
            };

        private static BookEvent NewTitleUpdated(Book book, DateTime atUtc, string description) =>
            NewUpdated(book, atUtc, EventTarget.BookTitle, description);

        private static BookEvent NewAuthorAdded(Book book, DateTime atUtc, string description) =>
            new()
            {
                BookId = book.Id,
                OccuredAt = atUtc,
                Target = EventTarget.BookAuthor,
                Type = EventType.Created,
                Description = description
            };

        private static BookEvent NewAuthorRemoved(Book book, DateTime atUtc, string description) =>
            new()
            {
                BookId = book.Id,
                OccuredAt = atUtc,
                Target = EventTarget.BookAuthor,
                Type = EventType.Deleted,
                Description = description
            };
    }
}
