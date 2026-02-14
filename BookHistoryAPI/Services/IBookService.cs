using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IBookService
    {
        Task<int> CreateAsync(BookDto dto);
        Task<BookDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateBookDto dto);
        Task<List<BookHistoryDto>> GetBookHistoryAsync(int bookId, BookHistoryQueryDto queryDto);
    }
}
