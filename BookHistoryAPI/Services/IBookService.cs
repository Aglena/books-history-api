using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IBookService
    {
        Task<int> CreateAsync(BookDto dto);
        Task<List<BookDto>> GetAll(BookQueryDto queryDto);
        Task<BookDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateBookDto dto);
    }
}
