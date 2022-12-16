using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("select")]
[WpFormsFieldType("radio")]
public class ChoicesField : FieldBase
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    [JsonPropertyName("choices")]
    public JsonElement RawChoices { get; set; }

    [JsonIgnore]
    public List<FieldChoice> Choices { get; } = new();

    public override bool Initialize( FormDefinition formDef )
    {
        if( !base.Initialize( formDef ) )
            return false;

        // choices are stored as JSON objects with invalid C# names
        // (they're the choice's index).
        if( RawChoices.ValueKind != JsonValueKind.Object )
            return false;

        var retVal = true;

        foreach( var choice in RawChoices.EnumerateObject() )
        {
            var temp = JsonSerializer.Deserialize<FieldChoice>( choice.Value.GetRawText(), _options );

            if( temp == null )
                retVal = false;
            else Choices.Add( temp );
        }

        return retVal;
    }
}
