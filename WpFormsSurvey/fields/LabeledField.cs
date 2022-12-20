using System.Text.Json.Serialization;

namespace WpFormsSurvey;

public class LabeledField : FieldBase
{
    protected LabeledField() { }

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}
