using BookHistoryApi.Domain.Entities;
using BookHistoryApi.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public static class BookEventQueryDtoValidator
    {
        internal static void Validate(BookEventQueryDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Target))
            {
                if (!Enum.IsDefined(typeof(EventTarget), dto.Target.Trim()))
                    throw new ValidationException($"Invalid event {nameof(dto.Target)} value \"{dto.Target}\"");
            }

            if (!string.IsNullOrWhiteSpace(dto.Type))
            {
                if (!Enum.IsDefined(typeof(EventType), dto.Type.Trim()))
                    throw new ValidationException($"Invalid event {nameof(dto.Type)} value \"{dto.Type}\"");
            }

            if (dto.OccuredFrom.HasValue && dto.OccuredTo.HasValue && dto.OccuredFrom.Value > dto.OccuredTo.Value)
                throw new ValidationException(
                    $"{nameof(dto.OccuredFrom)} ({dto.OccuredFrom.Value:O}) must be <= {nameof(dto.OccuredTo)} ({dto.OccuredTo.Value:O})");
        }
    }
}
