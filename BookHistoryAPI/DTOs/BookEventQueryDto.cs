namespace BookHistoryApi.DTOs
{
    public class BookEventQueryDto : QueryDto
    {
        public string? Description { get; set; }
        public string? Target { get; set; }
        public string? Type { get; set; }
        public DateTime? OccuredFrom { get; set; }
        public DateTime? OccuredTo { get; set; }

        public SortingField? OrderBy { get; set; }
    }
}