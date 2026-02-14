namespace BookHistoryApi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly PublishDate { get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<BookEvent> Events { get; set; } = new List<BookEvent>();
    }
}
