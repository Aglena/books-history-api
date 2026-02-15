using System.Text.Json.Serialization;

namespace BookHistoryApi.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventTarget
    {
        Book,
        BookAuthor,
        BookTitle,
        BookDescription,
        BookPublishDate,
    }
}
