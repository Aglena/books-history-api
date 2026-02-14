using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IHistoryService
    {
        Task<List<BookEventDto>> GetByBookIdAsync(int id, BookEventQueryDto query);
    }
}
