using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IBookService
    {
        Task<int> CreateAsync(BookDto dto);
        Task UpdateAsync(int id, BookDto dto);
    }
}
