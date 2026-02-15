using BookHistoryApi.Entities;

namespace BookHistoryApi.DTOs
{
    public class BookEventDto
    {
        public DateTime OccuredAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public EventTarget Target { get; set; }
        public EventType Type { get; set; }
    }
}
