using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IBookService
    {
        Task<int> CreateAsync(CreateBookDto dto);
        Task UpdateAsync(int id, UpdateBookDto dto);
    }
}
