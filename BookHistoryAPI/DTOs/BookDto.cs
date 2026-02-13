using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.DTOs
{
    public class BookDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }
}
