using BookHistoryApi.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Validation
{
    public static class BookDtoValidator
    {
        public static void ValidateForUpdate(UpdateBookDto dto)
        {
            if (dto is null)
                throw new ValidationException("Request body is required");

            if (string.IsNullOrWhiteSpace(dto.Title)
                    && string.IsNullOrWhiteSpace(dto.Description)
                    && dto.PublishDate == null
                    && dto.Authors == null)
                throw new ValidationException($"{nameof(UpdateBookDto)} cannot be empty");

            if (dto.Authors != null && dto.Authors.Any(a => string.IsNullOrWhiteSpace(a)))
                throw new ValidationException("Author name cannot be empty");
        }

        public static void ValidateForCreate(BookDto dto)
        {
            if (dto is null)
                throw new ValidationException("Request body is required");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title cannot be empty");

            if (dto.Authors.Any(a => string.IsNullOrWhiteSpace(a)))
                throw new ValidationException("Author name cannot be empty");
        }
    }
}
