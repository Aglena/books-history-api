namespace BookHistoryApi.DTOs
{
    public class BookQueryDto : QueryDto
    {
        public string? TitleOrDescription { get; set; }
        public string? Author { get; set; }
        public DateOnly? PublishedFrom { get; set; }
        public DateOnly? PublishedTo { get; set; }

        public BookSortingField? OrderBy { get; set; }
    }
}
