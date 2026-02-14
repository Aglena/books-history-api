using System.Text.Json.Serialization;

namespace BookHistoryApi.Entities
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
