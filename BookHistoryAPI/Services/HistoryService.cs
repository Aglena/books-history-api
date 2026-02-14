using BookHistoryApi.Data;
using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using BookHistoryApi.Exceptions;
using BookHistoryApi.Extensions;
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

            var events = _context.BookEvents
                .AsNoTracking()
                .Where(h => h.BookId == bookId);

            events = ApplyFiltering(events, queryDto);
            events = ApplyOrdering(events, queryDto);
            events = ApplyPagination(events, queryDto);

            return await events
                .Select(h => new BookEventDto
                {
                    OccuredAt = h.OccuredAt,
                    Description = h.Description
                })
                .ToListAsync();
        }


        private IQueryable<BookEvent> ApplyFiltering(IQueryable<BookEvent> events, BookEventQueryDto queryDto)
        {
            EventTarget? eventTarget = null;
            if (!string.IsNullOrWhiteSpace(queryDto.Target))
                eventTarget = Enum.Parse<EventTarget>(queryDto.Target.Trim(), ignoreCase: true);

            EventType? eventType = null;
            if (!string.IsNullOrWhiteSpace(queryDto.Type))
                eventType = Enum.Parse<EventType>(queryDto.Type.Trim(), ignoreCase: true);

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

            return events;
        }

        private IQueryable<BookEvent> ApplyOrdering(IQueryable<BookEvent> events, BookEventQueryDto queryDto)
        {
            var isAsc = queryDto.OrderDir.HasValue
                ? queryDto.OrderDir.Value == SortingOrder.Asc
                : false;

            switch (queryDto.OrderBy)
            {
                case SortingField.OccuredAt:
                    events = events
                        .ApplyOrder(e => e.OccuredAt, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case SortingField.EventTarget:
                    events = events
                        .ApplyOrder(e => e.Target, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case SortingField.EventType:
                    events = events
                        .ApplyOrder(e => e.Type, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case null:
                default:
                    events = events
                        .ApplyOrder(e => e.OccuredAt, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
            }

            return events;
        }

        private IQueryable<BookEvent> ApplyPagination(IQueryable<BookEvent> events, BookEventQueryDto queryDto)
        {
            return events.ApplyPaging(queryDto.Page, queryDto.PageSize);
        }
    }
}
