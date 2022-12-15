using System.Text.Json.Serialization;

namespace WpFormsSurvey;

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
            Values.AddRange( _rawValue.Split( '/' ).Select( x => x.Trim() ) );
        }
    }

    public List<string> Values { get; } = new();
}
