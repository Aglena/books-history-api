using System.Text.Json.Serialization;

namespace BookHistoryApi.Application.DTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortingOrder
    {
        Asc,
        Desc
    }
}
