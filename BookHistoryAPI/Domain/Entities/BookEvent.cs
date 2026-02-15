namespace BookHistoryApi.Domain.Entities
{
    public class BookEvent
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public DateTime OccuredAt { get; set; }
        public EventTarget Target { get; set; }
        public EventType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
