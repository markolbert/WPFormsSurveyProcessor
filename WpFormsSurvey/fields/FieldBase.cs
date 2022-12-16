using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class FieldBase
{
    public FormDefinition? Form { get; private set; }

    public virtual bool Initialize( FormDefinition formDef )
    {
        Form = formDef;
        return true;
    }

    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string FieldType { get; set; } = string.Empty;

    [JsonConverter(typeof(WpFormsBooleanConverter))]
    public bool SurveyField { get; set; }

    [JsonPropertyName("conditional_logic")]
    public int ConditionalLogic { get; set; }

    [JsonPropertyName("conditional_type")]
    public string ConditionalType { get; set; } = string.Empty;

    public List<List<FieldConditional>>? Conditionals { get; set; }
}