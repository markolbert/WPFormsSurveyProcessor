using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class FieldConditional
{
    [JsonPropertyName("field")]
    public int LinkedFieldId { get; set; }
    public string Operator { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public int LinkedChoiceId { get; set; }
}
