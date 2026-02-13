using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Services
{
    public class BookHistoryQuery
    {
        public string? Description { get; }
        public BookProperty? ChangedProperty { get; }
        public DateTime? ChangedFrom { get; }
        public DateTime? ChangedTo { get; }

        private BookHistoryQuery(
            string? description,
            BookProperty? propertyChanged,
            DateTime? changedFrom,
            DateTime? changedTo)
        {
            Description = description;
            ChangedProperty = propertyChanged;
            ChangedFrom = changedFrom;
            ChangedTo = changedTo;
        }


        public static BookHistoryQuery FromDto(BookHistoryQueryDto dto)
        {
            BookProperty? property = null;
            if (!string.IsNullOrWhiteSpace(dto.ChangedProperty))
            {
                if (!Enum.TryParse(dto.ChangedProperty.Trim(), ignoreCase: true, out BookProperty parsed))
                    throw new ValidationException($"Invalid {nameof(dto.ChangedProperty)} value \"{dto.ChangedProperty}\"");
                property = parsed;
            }

            var from = dto.ChangedFrom?.Date;
            var to = dto.ChangedTo?.Date;

            if (from.HasValue && to.HasValue && from.Value > to.Value)
                throw new ValidationException(
                    $"{nameof(dto.ChangedFrom)} ({from:O}) must be <= {nameof(dto.ChangedTo)} ({to:O})");

            return new BookHistoryQuery(
                string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                property,
                from,
                to);
        }
    }
}
