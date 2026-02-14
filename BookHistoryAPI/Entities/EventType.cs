using System.Text.Json.Serialization;

namespace BookHistoryApi.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventType
    {
        Created,
        Updated,
        Deleted
    }
}
