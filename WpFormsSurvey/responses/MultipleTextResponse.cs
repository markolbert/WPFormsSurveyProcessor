using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class MultipleTextResponse : ResponseBase
{
    [JsonPropertyName("value")]
    public string RawValue { get; set; } = string.Empty;

    public List<string> Values { get; set; } = new();
}
