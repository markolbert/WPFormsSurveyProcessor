using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("pagebreak")]
public class PageBreakField : FieldBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
