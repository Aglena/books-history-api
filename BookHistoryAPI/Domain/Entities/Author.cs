using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
