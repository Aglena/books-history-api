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
            BookHistoryDtoValidator.ValidateForCreate(dto);

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
            BookHistoryDtoValidator.ValidateForUpdate(dto);

            var book = await _context.Books
                .Include(b => b.ChangeHistory)
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

        public async Task<List<BookHistoryDto>> GetBookHistoryAsync(int bookId, BookHistoryQueryDto queryDto)
        {
            BookHistoryQueryDtoValidator.Validate(queryDto);

            var exists = await _context.Books
                .AnyAsync(b => b.Id == bookId);

            if (!exists)
                throw new BookNotFoundException($"Book with id {bookId} not found");

            BookProperty? changedProperty = null;
            if (!string.IsNullOrWhiteSpace(queryDto.ChangedProperty))
                Enum.Parse<BookProperty>(queryDto.ChangedProperty.Trim());

            IQueryable<BookHistoryEntry> historyEntries = _context.BookChangeHistories
                .AsNoTracking()
                .Where(h => h.BookId == bookId);

            if (changedProperty.HasValue)
                historyEntries = historyEntries.Where(h => h.ChangedProperty == changedProperty.Value);

            if (queryDto.ChangedFrom.HasValue)
                historyEntries = historyEntries.Where(h => h.ChangeDate >= queryDto.ChangedFrom.Value);

            if (queryDto.ChangedTo.HasValue)
                historyEntries = historyEntries.Where(h => h.ChangeDate <= queryDto.ChangedTo.Value);

            if (!string.IsNullOrWhiteSpace(queryDto.Description))
                historyEntries = historyEntries.Where(h => h.Description.Contains(queryDto.Description));

            switch (queryDto.OrderBy)
            {
                case SortingField.ChangeDate:
                    historyEntries = queryDto.OrderDir == SortingOrder.Asc 
                        ? historyEntries
                            .OrderBy(h => h.ChangeDate)
                            .ThenBy(h => h.Id)
                        : historyEntries
                            .OrderByDescending(h => h.ChangeDate)
                            .ThenByDescending(h => h.Id);
                    break;
                case null:
                default:
                    historyEntries = queryDto.OrderDir == SortingOrder.Asc
                        ? historyEntries
                            .OrderBy(h => h.ChangeDate)
                            .ThenBy(h => h.Id)
                        : historyEntries
                            .OrderByDescending(h => h.ChangeDate)
                            .ThenByDescending(h => h.Id);
                    break;
            }

            historyEntries = historyEntries
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize);

            return await historyEntries
                .Select(h => new BookHistoryDto
                {
                    ChangeDate = h.ChangeDate,
                    Description = h.Description
                })
                .ToListAsync();
        }


        private void UpdateTitleIfChanged(Book book, string? title, DateTime now)
        {
            var newTitle = title?.Trim();

            if (!string.IsNullOrEmpty(newTitle) && book.Title != newTitle)
            {
                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Title was changed to \"{newTitle}\"",
                    ChangedProperty = BookProperty.Title,
                    ChangeDate = now
                });

                book.Title = newTitle;
            }
        }

        private void UpdateDescriptionIfChanged(Book book, string? description, DateTime now)
        {
            var newDescription = description?.Trim();

            if (!string.IsNullOrEmpty(newDescription) && book.Description != newDescription)
            {
                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Description was changed to \"{newDescription}\"",
                    ChangedProperty = BookProperty.Description,
                    ChangeDate = now
                });

                book.Description = newDescription;
            }
        }

        private void UpdatePublishDateIfChanged(Book book, DateTime? publishDate, DateTime now)
        {
            if (publishDate.HasValue && book.PublishDate != publishDate)
            {
                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Publish date was changed to \"{publishDate.Value}\"",
                    ChangedProperty = BookProperty.PublishDate,
                    ChangeDate = now
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

                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Author \"{author.Name}\" was removed",
                    ChangedProperty = BookProperty.Author,
                    ChangeDate = now
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

                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Author \"{existingAuthor.Name}\" was added",
                    ChangedProperty = BookProperty.Author,
                    ChangeDate = now
                });
            }
        }
    }
}
