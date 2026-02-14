using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using BookHistoryApi.Extensions;

namespace BookHistoryApi.Querying
{
    public static class BookQuery
    {
        public static IQueryable<Book> Apply(IQueryable<Book> q, BookQueryDto dto)
        {
            q = ApplyFiltering(q, dto);
            q = ApplyOrdering(q, dto);
            q = ApplyPaging(q, dto);
            return q;
        }

        private static IQueryable<Book> ApplyFiltering(IQueryable<Book> books, BookQueryDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.TitleOrDescription))
                books = books.Where(
                    h => h.Title.Contains(dto.TitleOrDescription) || h.Description.Contains(dto.TitleOrDescription));

            if (!string.IsNullOrWhiteSpace(dto.Author))
                books = books.Where(h => h.Authors.Any(a => a.Name.Contains(dto.Author)));

            if (dto.PublishedFrom.HasValue)
                books = books.Where(h => h.PublishDate >= dto.PublishedFrom.Value);

            if (dto.PublishedTo.HasValue)
                books = books.Where(h => h.PublishDate <= dto.PublishedTo.Value);

            return books;
        }

        private static IQueryable<Book> ApplyOrdering(IQueryable<Book> books, BookQueryDto dto)
        {
            var isAsc = dto.OrderDir.HasValue
                ? dto.OrderDir.Value == SortingOrder.Asc
                : true;

            switch (dto.OrderBy)
            {
                case BookSortingField.Title:
                    books = books
                        .ApplyOrder(e => e.Title, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case BookSortingField.PublishDate:
                    books = books
                        .ApplyOrder(e => e.PublishDate, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
                case null:
                default:
                    books = books
                        .ApplyOrder(e => e.PublishDate, isAsc)
                        .ApplyOrder(e => e.Id, isAsc, true);
                    break;
            }

            return books;
        }

        private static IQueryable<Book> ApplyPaging(IQueryable<Book> q, BookQueryDto dto)
        {
            return q.ApplyPaging(dto.Page, dto.PageSize);
        }
    }
}
