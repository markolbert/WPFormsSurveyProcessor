using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonFieldName("html")]
public class HtmlField : FieldBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
