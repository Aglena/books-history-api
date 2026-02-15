using BookHistoryApi.Domain.Entities;
using BookHistoryApi.DTOs;
using BookHistoryApi.Extensions;
using Microsoft.EntityFrameworkCore;

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
            {
                var pattern = $"%{dto.TitleOrDescription.Trim()}%";
                books = books.Where(b =>
                    EF.Functions.Like(b.Title, pattern) ||
                    EF.Functions.Like(b.Description, pattern));
            }

            if (!string.IsNullOrWhiteSpace(dto.Author))
            {
                var pattern = $"%{dto.Author.Trim()}%";
                books = books.Where(b =>
                    b.Authors.Any(a => EF.Functions.Like(a.Name, pattern)));
            }

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
