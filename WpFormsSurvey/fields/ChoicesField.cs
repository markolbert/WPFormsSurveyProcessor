using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("select")]
[WpFormsFieldType("radio")]
public class ChoicesField : FieldBase
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    [JsonPropertyName("choices")]
    public JsonElement RawChoices { get; set; }

    [JsonIgnore]
    public List<FieldChoice> Choices { get; } = new();

    public override bool Initialize( Form formDef )
    {
        if( !base.Initialize( formDef ) )
            return false;

        // for some strange reason, some WpForms forms have the Fields object as a JsonArray, and some 
        // have it as a JsonObject. We need to accomodate both
        var fieldDefEnumerator = RawChoices.ValueKind switch
        {
            JsonValueKind.Array => EnumerateFieldsArray<FieldChoice>(RawChoices, _options),
            JsonValueKind.Object => EnumerateFieldsObject<FieldChoice>( RawChoices, _options ),
            _ => UnsupportedEnumerator<FieldChoice>(RawChoices.ValueKind)
        };

        var retVal = true;

        foreach( var choice in fieldDefEnumerator )
        {
            if( choice == null )
                retVal = false;
            else Choices.Add( choice );
        }

        return retVal;
    }
}
