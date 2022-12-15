using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class ResponseBase : IJsonField
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string FieldLabel { get; set; } = string.Empty;

    public virtual bool Initialize() => true;
}
