using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.DTOs
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }
}
