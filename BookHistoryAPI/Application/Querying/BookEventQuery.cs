using BookHistoryApi.Application.DTOs;
using BookHistoryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookHistoryApi.Application.Querying
{
    public static class BookEventQuery
    {
        public static IQueryable<BookEvent> Apply(IQueryable<BookEvent> q, BookEventQueryDto dto)
        {
            q = ApplyFiltering(q, dto);
            q = ApplyOrdering(q, dto);
            q = ApplyPaging(q, dto);
            return q;
        }

        private static IQueryable<BookEvent> ApplyFiltering(IQueryable<BookEvent> events, BookEventQueryDto dto)
        {
            EventTarget? eventTarget = null;
            if (!string.IsNullOrWhiteSpace(dto.Target))
                eventTarget = Enum.Parse<EventTarget>(dto.Target.Trim(), ignoreCase: true);

            EventType? eventType = null;
            if (!string.IsNullOrWhiteSpace(dto.Type))
                eventType = Enum.Parse<EventType>(dto.Type.Trim(), ignoreCase: true);

            if (eventTarget.HasValue)
                events = events.Where(h => h.Target == eventTarget.Value);

            if (eventType.HasValue)
                events = events.Where(h => h.Type == eventType.Value);

            if (dto.OccuredFrom.HasValue)
                events = events.Where(h => h.OccuredAt >= dto.OccuredFrom.Value);

            if (dto.OccuredTo.HasValue)
                events = events.Where(h => h.OccuredAt <= dto.OccuredTo.Value);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var pattern = $"%{dto.Description.Trim()}%";
                events = events.Where(e => EF.Functions.Like(e.Description, pattern));
            }

            return events;
        }

        private static IQueryable<BookEvent> ApplyOrdering(IQueryable<BookEvent> events, BookEventQueryDto dto)
        {
            var isAsc = dto.OrderDir.HasValue
                ? dto.OrderDir.Value == SortingOrder.Asc
                : false;

            switch (dto.OrderBy)
            {
                case BookEventSortingField.OccuredAt:
                    events = events
                        .ApplyOrder(e => e.OccuredAt, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case BookEventSortingField.EventTarget:
                    events = events
                        .ApplyOrder(e => e.Target, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case BookEventSortingField.EventType:
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

        private static IQueryable<BookEvent> ApplyPaging(IQueryable<BookEvent> q, BookEventQueryDto dto)
        {
            return q.ApplyPaging(dto.Page, dto.PageSize);
        }
    }
}
