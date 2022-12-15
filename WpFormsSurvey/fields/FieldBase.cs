using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;

namespace WPFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class FieldBase : IJsonField
{
    protected FieldBase()
    {
    }

    public virtual bool IsValid => true;
    public virtual bool Initialize() => true;

    public int Id { get; set; }

    [JsonPropertyName("conditional_logic")]
    public int ConditionalLogic { get; set; }

    [JsonPropertyName("conditional_type")]
    public string ConditionalType { get; set; } = string.Empty;

    public List<List<FieldConditional>>? Conditionals { get; set; }
}