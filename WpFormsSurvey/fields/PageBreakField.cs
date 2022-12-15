using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class PageBreakField : FieldBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
