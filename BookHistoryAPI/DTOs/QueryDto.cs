using System.ComponentModel.DataAnnotations;

namespace BookHistoryApi.DTOs
{
    public abstract class QueryDto
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
