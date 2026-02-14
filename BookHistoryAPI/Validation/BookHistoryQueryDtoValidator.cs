using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public static class BookHistoryQueryDtoValidator
    {
        internal static void Validate(BookHistoryQueryDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ChangedProperty))
            {
                if (!Enum.IsDefined(typeof(BookProperty), dto.ChangedProperty.Trim()))
                    throw new ValidationException($"Invalid {nameof(dto.ChangedProperty)} value \"{dto.ChangedProperty}\"");
            }

            if (dto.ChangedFrom.HasValue && dto.ChangedTo.HasValue && dto.ChangedFrom.Value > dto.ChangedTo.Value)
                throw new ValidationException(
                    $"{nameof(dto.ChangedFrom)} ({dto.ChangedFrom.Value:O}) must be <= {nameof(dto.ChangedTo)} ({dto.ChangedTo.Value:O})");
        }
    }
}
