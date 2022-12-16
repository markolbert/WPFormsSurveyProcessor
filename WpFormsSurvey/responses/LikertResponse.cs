using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("likert-scale")]
public class LikertResponse : ResponseBase
{
    [JsonPropertyName("value_raw")]
    public string? RawValue { get; set; }

    [JsonIgnore]
    public List<LikertScore> Scores { get; } = new();
}
