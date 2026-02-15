using BookHistoryApi.Application.DTOs;

namespace BookHistoryApi.Application.Services
{
    public interface IBookService
    {
        Task<int> CreateAsync(BookDto dto);
        Task<List<BookDto>> GetAll(BookQueryDto queryDto);
        Task<BookDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateBookDto dto);
    }
}
