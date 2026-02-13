namespace BookHistoryApi.DTOs
{
    public class BookHistoryQueryDto : QueryDto
    {
        public string? Description { get; set; }
        public string? ChangedProperty { get; set; }
        public DateTime? ChangedFrom { get; set; }
        public DateTime? ChangedTo { get; set; }

        public SortingField? OrderBy { get; set; }
    }
}