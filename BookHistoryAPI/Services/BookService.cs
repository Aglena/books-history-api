using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using BookHistoryApi.Exceptions;
using BookHistoryApi.Validation;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Services
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
            await _context.SaveChangesAsync();

            return book.Id;
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

        public async Task<List<BookEventDto>> GetBookHistoryAsync(int bookId, BookEventQueryDto queryDto)
        {
            BookEventQueryDtoValidator.Validate(queryDto);

            var exists = await _context.Books
                .AnyAsync(b => b.Id == bookId);

            if (!exists)
                throw new BookNotFoundException($"Book with id {bookId} not found");

            EventTarget? changedProperty = null;
            if (!string.IsNullOrWhiteSpace(queryDto.ChangedProperty))
                changedProperty = Enum.Parse<EventTarget>(queryDto.ChangedProperty.Trim(), ignoreCase: true);

            IQueryable<BookEvent> events = _context.BookEvents
                .AsNoTracking()
                .Where(h => h.BookId == bookId);

            if (changedProperty.HasValue)
                events = events.Where(h => h.Target == changedProperty.Value);

            if (queryDto.OccuredFrom.HasValue)
                events = events.Where(h => h.OccuredAt >= queryDto.OccuredFrom.Value);

            if (queryDto.OccuredTo.HasValue)
                events = events.Where(h => h.OccuredAt <= queryDto.OccuredTo.Value);

            if (!string.IsNullOrWhiteSpace(queryDto.Description))
                events = events.Where(h => h.Description.Contains(queryDto.Description));

            switch (queryDto.OrderBy)
            {
                case SortingField.OccuredAt:
                    events = queryDto.OrderDir == SortingOrder.Asc 
                        ? events
                            .OrderBy(h => h.OccuredAt)
                            .ThenBy(h => h.Id)
                        : events
                            .OrderByDescending(h => h.OccuredAt)
                            .ThenByDescending(h => h.Id);
                    break;
                case SortingField.EventTarget:
                    events = queryDto.OrderDir == SortingOrder.Asc
                        ? events
                            .OrderBy(h => h.Target)
                            .ThenBy(h => h.Id)
                        : events
                            .OrderByDescending(h => h.Target)
                            .ThenByDescending(h => h.Id);
                    break;
                case SortingField.EventType:
                    events = queryDto.OrderDir == SortingOrder.Asc
                        ? events
                            .OrderBy(h => h.Type)
                            .ThenBy(h => h.Id)
                        : events
                            .OrderByDescending(h => h.Type)
                            .ThenByDescending(h => h.Id);
                    break;

                case null:
                default:
                    events = queryDto.OrderDir == SortingOrder.Asc
                        ? events
                            .OrderBy(h => h.OccuredAt)
                            .ThenBy(h => h.Id)
                        : events
                            .OrderByDescending(h => h.OccuredAt)
                            .ThenByDescending(h => h.Id);
                    break;
            }

            events = events
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize);

            return await events
                .Select(h => new BookEventDto
                {
                    OccuredAt = h.OccuredAt,
                    Description = h.Description
                })
                .ToListAsync();
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
