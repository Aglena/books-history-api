using BookHistoryApi.DTOs;

namespace BookHistoryApi.Services
{
    public interface IBookEventsService
    {
        Task<List<BookEventDto>> GetBookHistoryAsync(int bookId, BookEventQueryDto queryDto);
    }
}
