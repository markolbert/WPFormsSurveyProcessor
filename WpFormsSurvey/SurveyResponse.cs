using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class SurveyResponse
{
    private string _rawFields = string.Empty;

    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }

    [JsonPropertyName("form_id")]
    public int FormId { get; set; }
    public DateTime Date { get; set; }

    [JsonPropertyName("date_modified")]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; }= string.Empty;
    public string Status { get; set; } = string.Empty;

    [ JsonPropertyName( "fields" ) ]
    public string RawFields
    {
        get => _rawFields;

        set
        {
            _rawFields = value;
            ParseFields( value );
        }
    }

    private void ParseFields( string raw )
    {
        Responses.Clear();

        if( string.IsNullOrEmpty( raw ) )
            return;
    }

    public List<ResponseBase> Responses { get; } = new();
}