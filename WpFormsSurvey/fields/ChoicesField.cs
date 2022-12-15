using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class ChoicesField : FieldBase
{
    protected ChoicesField()
    {
    }

    [JsonPropertyName("choices")]
    public JsonElement RawChoices { get; set; }

    [JsonIgnore]
    public List<FieldChoice> Choices { get; } = new();
}
