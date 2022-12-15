using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonFieldName("pagebreak")]
public class PageBreakField : FieldBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
