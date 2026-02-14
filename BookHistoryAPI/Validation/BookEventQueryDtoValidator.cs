using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public static class BookEventQueryDtoValidator
    {
        internal static void Validate(BookEventQueryDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ChangedProperty))
            {
                if (!Enum.IsDefined(typeof(EventTarget), dto.ChangedProperty.Trim()))
                    throw new ValidationException($"Invalid {nameof(dto.ChangedProperty)} value \"{dto.ChangedProperty}\"");
            }

            if (dto.OccuredFrom.HasValue && dto.OccuredTo.HasValue && dto.OccuredFrom.Value > dto.OccuredTo.Value)
                throw new ValidationException(
                    $"{nameof(dto.OccuredFrom)} ({dto.OccuredFrom.Value:O}) must be <= {nameof(dto.OccuredTo)} ({dto.OccuredTo.Value:O})");
        }
    }
}
