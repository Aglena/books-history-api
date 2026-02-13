using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using BookHistoryApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title cannot be empty");

            if (dto.Authors.Any(a => string.IsNullOrWhiteSpace(a)))
                throw new ValidationException("Author name cannot be empty");

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

        public async Task UpdateAsync(int id, BookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title cannot be empty");

            if (dto.Authors.Any(a => string.IsNullOrWhiteSpace(a)))
                throw new ValidationException("Author name cannot be empty");

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


        private void UpdateTitleIfChanged(Book book, string title, DateTime now)
        {
            var newTitle = title.Trim();

            if (book.Title != newTitle)
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

        private void UpdateDescriptionIfChanged(Book book, string description, DateTime now)
        {
            var newDescription = description?.Trim() ?? string.Empty;
            if (book.Description != newDescription)
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

        private void UpdatePublishDateIfChanged(Book book, DateTime publishDate, DateTime now)
        {
            if (book.PublishDate != publishDate)
            {
                book.ChangeHistory.Add(new BookHistoryEntry
                {
                    BookId = book.Id,
                    Description = $"Publish date was changed to \"{publishDate}\"",
                    ChangedProperty = BookProperty.PublishDate,
                    ChangeDate = now
                });

                book.PublishDate = publishDate;
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
                    ChangedProperty = BookProperty.AuthorName,
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
                    ChangedProperty = BookProperty.AuthorName,
                    ChangeDate = now
                });
            }
        }
    }
}
