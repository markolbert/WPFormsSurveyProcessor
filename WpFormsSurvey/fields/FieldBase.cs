using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class FieldBase : IJsonField
{
    protected FieldBase()
    {
    }

    public virtual bool Initialize() => true;

    public int Id { get; set; }

    [JsonConverter(typeof(WpFormsBooleanConverter))]
    public bool SurveyField { get; set; }

    [JsonPropertyName("conditional_logic")]
    public int ConditionalLogic { get; set; }

    [JsonPropertyName("conditional_type")]
    public string ConditionalType { get; set; } = string.Empty;

    public List<List<FieldConditional>>? Conditionals { get; set; }
}