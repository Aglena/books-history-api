using System.Text.Json.Serialization;

namespace BookHistoryApi.DTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookSortingField
    {
        Title,
        PublishDate
    }
}
