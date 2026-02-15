using BookHistoryApi.Application.DTOs;
using BookHistoryApi.Application.Querying;
using BookHistoryApi.Application.Validation;
using BookHistoryApi.Data;
using BookHistoryApi.Domain.Entities;
using BookHistoryApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Application.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<int> CreateAsync(BookDto dto)
        {
            BookDtoValidator.ValidateForCreate(dto);

            var book = new Book
            {
                Title = dto.Title.Trim(),
                Description = dto.Description,
                PublishDate = dto.PublishDate,
                Authors = dto.Authors
                    .Select(a => new Author { Name = a.Trim() })
                    .ToList()
            };

            _context.Books.Add(book);

            _context.BookEvents.Add(new BookEvent
            {
                Book = book,
                OccuredAt = DateTime.UtcNow,
                Target = EventTarget.Book,
                Type = EventType.Created,
                Description = $"Book \"{book.Title}\" created"
            });

            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task<List<BookDto>> GetAll(BookQueryDto queryDto)
        {
            BookQueryDtoValidator.Validate(queryDto);

            var books = _context.Books.AsNoTracking();

            books = BookQuery.Apply(books, queryDto);

            return await books
                .Select(b => new BookDto
                {
                    Title = b.Title,
                    Description = b.Description,
                    PublishDate = b.PublishDate,
                    Authors = b.Authors
                        .Select(a => a.Name)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<BookDto> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new BookNotFoundException($"Book with id {id} not found");

            return new BookDto
            {
                Title = book.Title,
                Description = book.Description,
                PublishDate = book.PublishDate,
                Authors = book.Authors
                    .Select(a => a.Name)
                    .ToList()
            };
        }

        public async Task UpdateAsync(int id, UpdateBookDto dto)
        {
            BookDtoValidator.ValidateForUpdate(dto);

            var book = await _context.Books
                .Include(b => b.Events)
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new BookNotFoundException($"Book with id {id} not found");

            var now = DateTime.UtcNow;

            UpdateTitleIfChanged(book, dto.Title, now);
            UpdateDescriptionIfChanged(book, dto.Description, now);
            UpdatePublishDateIfChanged(book, dto.PublishDate, now);
            await UpdateAuthorsIfChanged(book, dto.Authors, now);

            await _context.SaveChangesAsync();
        }


        private void UpdateTitleIfChanged(Book book, string? title, DateTime now)
        {
            var newTitle = title?.Trim();

            if (!string.IsNullOrEmpty(newTitle) && book.Title != newTitle)
            {
                book.Events.Add(new BookEvent
                {
                    BookId = book.Id,
                    Description = $"Title was changed to \"{newTitle}\"",
                    Target = EventTarget.BookTitle,
                    Type = EventType.Updated,
                    OccuredAt = now
                });

                book.Title = newTitle;
            }
        }

        private void UpdateDescriptionIfChanged(Book book, string? description, DateTime now)
        {
            var newDescription = description?.Trim();

            if (!string.IsNullOrEmpty(newDescription) && book.Description != newDescription)
            {
                book.Events.Add(new BookEvent
                {
                    BookId = book.Id,
                    Description = $"Description was changed to \"{newDescription}\"",
                    Target = EventTarget.BookDescription,
                    Type = EventType.Updated,
                    OccuredAt = now
                });

                book.Description = newDescription;
            }
        }

        private void UpdatePublishDateIfChanged(Book book, DateOnly? publishDate, DateTime now)
        {
            if (publishDate.HasValue && book.PublishDate != publishDate)
            {
                book.Events.Add(new BookEvent
                {
                    BookId = book.Id,
                    Description = $"Publish date was changed to \"{publishDate.Value}\"",
                    Target = EventTarget.BookPublishDate,
                    Type = EventType.Updated,
                    OccuredAt = now
                });

                book.PublishDate = publishDate.Value;
            }
        }

        private async Task UpdateAuthorsIfChanged(Book book, List<string>? authorNames, DateTime now)
        {
            var newAuthorNames = authorNames?
                .Select(a => a.Trim())
                .Distinct()
                .ToList() ?? new List<string>();

            var currentAuthorNames = book.Authors
                .Select(a => a.Name)
                .ToList();

            var authorsToRemove = book.Authors
                .Where(a => !newAuthorNames.Contains(a.Name))
                .ToList();

            foreach (var author in authorsToRemove)
            {
                book.Authors.Remove(author);

                book.Events.Add(new BookEvent
                {
                    BookId = book.Id,
                    Description = $"Author \"{author.Name}\" was removed",
                    Target = EventTarget.BookAuthor,
                    Type = EventType.Deleted,
                    OccuredAt = now
                });

                var isUsedElsewhere = await _context.Books
                    .AnyAsync(b => b.Id != book.Id && b.Authors.Any(a => a.Id == author.Id));

                if (!isUsedElsewhere)
                {
                    _context.Authors.Remove(author);
                }
            }

            var authorsToAdd = newAuthorNames
                .Where(name => !currentAuthorNames.Contains(name))
                .ToList();

            foreach (var authorName in authorsToAdd)
            {
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Name == authorName);

                if (existingAuthor == null)
                {
                    existingAuthor = new Author
                    {
                        Name = authorName
                    };

                    _context.Authors.Add(existingAuthor);
                }

                book.Authors.Add(existingAuthor);

                book.Events.Add(new BookEvent
                {
                    BookId = book.Id,
                    Description = $"Author \"{existingAuthor.Name}\" was added",
                    Target = EventTarget.BookAuthor,
                    Type = EventType.Created,
                    OccuredAt = now
                });
            }
        }
    }
}
