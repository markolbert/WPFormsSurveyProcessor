using System.Text.Json.Serialization;

namespace WPFormsSurvey;

[JsonFieldName("html")]
public class HtmlField : FieldBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
