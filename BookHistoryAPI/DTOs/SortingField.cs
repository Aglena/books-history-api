using System.Text.Json.Serialization;

namespace BookHistoryApi.DTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortingField
    {
        OccuredAt,
        EventTarget,
        EventType
    }
}