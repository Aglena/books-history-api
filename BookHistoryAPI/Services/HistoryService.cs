using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using BookHistoryApi.Exceptions;
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


        public async Task<List<BookEventDto>> GetByBookIdAsync(int bookId, BookEventQueryDto queryDto)
        {
            BookEventQueryDtoValidator.Validate(queryDto);

            var exists = await _context.Books
                .AnyAsync(b => b.Id == bookId);

            if (!exists)
                throw new BookNotFoundException($"Book with id {bookId} not found");

            EventTarget? eventTarget = null;
            if (!string.IsNullOrWhiteSpace(queryDto.Target))
                eventTarget = Enum.Parse<EventTarget>(queryDto.Target.Trim(), ignoreCase: true);

            EventType? eventType = null;
            if (!string.IsNullOrWhiteSpace(queryDto.Type))
                eventType = Enum.Parse<EventType>(queryDto.Type.Trim(), ignoreCase: true);

            IQueryable<BookEvent> events = _context.BookEvents
                .AsNoTracking()
                .Where(h => h.BookId == bookId);

            if (eventTarget.HasValue)
                events = events.Where(h => h.Target == eventTarget.Value);

            if (eventType.HasValue)
                events = events.Where(h => h.Type == eventType.Value);

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
    }
}
