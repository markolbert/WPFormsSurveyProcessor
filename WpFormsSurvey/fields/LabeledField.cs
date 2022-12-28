using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

public class LabeledField : FieldBase
{
    protected LabeledField() { }

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}
