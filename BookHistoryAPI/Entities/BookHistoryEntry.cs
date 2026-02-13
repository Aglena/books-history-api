namespace BookHistoryApi.Entities
{
    public class BookHistoryEntry
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public DateTime ChangeDate { get; set; }
        public BookProperty ChangedProperty { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
