namespace BookHistoryApi.DTOs
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? PublishDate { get; set; }
        public List<string>? Authors { get; set; }
    }
}
