using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;

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
    }
}
