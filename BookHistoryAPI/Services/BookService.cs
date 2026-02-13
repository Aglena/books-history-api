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


        public async Task<int> CreateAsync(CreateBookDto dto)
        {
            var title = dto.Title.Trim();

            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Title cannot be empty");

            var book = new Book
            {
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                PublishDate = dto.PublishDate,
                Authors = dto.Authors
                    .Select(a => new Author { Name = a })
                    .ToList()
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task UpdateAsync(int id, UpdateBookDto dto)
        {
            var book = await _context.Books
                .Include(b => b.ChangeHistory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new BookNotFoundException($"Book with id {id} not found");

            // Update logic
        }
    }
}
