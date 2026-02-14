using BookHistoryApi.DTOs;
using BookHistoryApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public class BookQueryDtoValidator
    {
        public static void Validate(BookQueryDto dto)
        {
            if (dto.PublishedFrom.HasValue && dto.PublishedTo.HasValue && dto.PublishedFrom.Value > dto.PublishedTo.Value)
                throw new ValidationException(
                    $"{nameof(dto.PublishedFrom)} ({dto.PublishedFrom.Value:O}) must be <= {nameof(dto.PublishedTo)} ({dto.PublishedTo.Value:O})");
        }
    }
}
