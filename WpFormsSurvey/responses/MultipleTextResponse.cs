using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("checkbox")]
public class MultipleTextResponse : ResponseBase
{
    private string _rawValue = string.Empty;

    [ JsonPropertyName( "value" ) ]
    public string RawValue
    {
        get => _rawValue;

        set
        {
            _rawValue = value;

            Values.Clear();
            Values.AddRange( _rawValue.Split( "\n" ).Select( x => x.Trim() ) );
        }
    }

    [JsonIgnore]
    public List<string> Values { get; } = new();
}
