using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class FormResponse
{
    private string _rawFields = string.Empty;

    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }

    [JsonPropertyName("form_id")]
    public int FormId { get; set; }

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyName("date_modified")]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; }= string.Empty;
    public string Status { get; set; } = string.Empty;

    [ JsonPropertyName( "fields" ) ]
    public JsonElement RawResponses { get; set; }

    public List<ResponseBase> Responses { get; } = new();
}