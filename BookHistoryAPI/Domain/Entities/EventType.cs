using System.Text.Json.Serialization;

namespace BookHistoryApi.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventType
    {
        Created,
        Updated,
        Deleted
    }
}
