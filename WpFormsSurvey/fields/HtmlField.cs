using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("html")]
public class HtmlField : FieldBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
