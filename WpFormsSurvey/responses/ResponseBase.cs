using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class ResponseBase 
{
    [JsonPropertyName("id")]
    public int FieldId { get; set; }

    [JsonPropertyName("name")]
    public string FieldLabel { get; set; } = string.Empty;
}
