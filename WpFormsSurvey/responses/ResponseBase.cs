using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class ResponseBase : JsonField
{
    protected ResponseBase()
    {
    }

    [JsonPropertyName("id")]
    public int FieldId { get; set; }

    [JsonPropertyName("name")]
    public string FieldLabel { get; set; } = string.Empty;

    public virtual bool Initialize() => true;
}
