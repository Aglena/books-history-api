using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Exceptions;
using BookHistoryApi.Querying;
using BookHistoryApi.Validation;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;

        public HistoryService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<BookEventDto>> GetAll(BookEventQueryDto queryDto)
        {
            BookEventQueryDtoValidator.Validate(queryDto);

            var events = _context.BookEvents.AsNoTracking();

            events = BookEventQuery.Apply(events, queryDto);

            return await events
                .Select(h => new BookEventDto
                {
                    OccuredAt = h.OccuredAt,
                    Description = h.Description,
                    Target = h.Target,
                    Type = h.Type
                })
                .ToListAsync();
        }

        public async Task<List<BookEventDto>> GetByBookIdAsync(int bookId, BookEventQueryDto queryDto)
        {
            BookEventQueryDtoValidator.Validate(queryDto);

            var exists = await _context.Books
                .AnyAsync(b => b.Id == bookId);

            if (!exists)
                throw new BookNotFoundException($"Book with id {bookId} not found");

            var events = _context.BookEvents
                .AsNoTracking()
                .Where(h => h.BookId == bookId);

            events = BookEventQuery.Apply(events, queryDto);

            return await events
                .Select(h => new BookEventDto
                {
                    OccuredAt = h.OccuredAt,
                    Description = h.Description,
                    Target = h.Target,
                    Type = h.Type
                })
                .ToListAsync();
        }
    }
}
