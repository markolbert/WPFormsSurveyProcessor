using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("html")]
public class HtmlField : FieldBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
