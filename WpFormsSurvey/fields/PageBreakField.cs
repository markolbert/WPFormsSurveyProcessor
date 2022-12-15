using System.Text.Json.Serialization;

namespace WPFormsSurvey;

[JsonFieldName("pagebreak")]
public class PageBreakField : FieldBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
