using BookHistoryApi.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public static class BookHistoryDtoValidator
    {
        public static void Validate(BookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title cannot be empty");

            if (dto.Authors.Any(a => string.IsNullOrWhiteSpace(a)))
                throw new ValidationException("Author name cannot be empty");
        }
    }
}
