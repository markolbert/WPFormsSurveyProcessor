using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class IndividualSubmission
{
    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }

    [JsonPropertyName("form_id")]
    public int FormId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("user_uuid")]
    [JsonConverter(typeof(WpGuidConverter))]
    public Guid UserGuid { get; set; }
        
    [JsonConverter(typeof(WpDateTimeConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyName("date_modified")]
    [JsonConverter(typeof(WpDateTimeConverter))]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public string? Fields { get; set; }

    [JsonIgnore]
    public List<ResponseBase> Responses { get; set; } = new();
}
