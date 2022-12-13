using System.Net;
using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class WpSurveyPost
{
    private string _rawContent = string.Empty;

    public int Id { get; set; }

    [JsonPropertyName("post_author")]
    public string PostAuthor { get; set; } = string.Empty;

    [JsonPropertyName("post_date")]
    public DateTime PostDate { get; set; }

    [JsonPropertyName("post_date_gmt")]
    public DateTime PostDateGmt { get; set; }

    [ JsonPropertyName( "post_content" ) ]
    public string RawPostContent
    {
        get => _rawContent;

        set
        {
            _rawContent = value;
            ParseContent( value );
        }
    }

    private void ParseContent( string raw )
    {
        Fields.Clear();

        if( string.IsNullOrEmpty( raw ) )
            return;
    }

    [JsonIgnore]
    public List<FieldBase> Fields { get; set; } = new();

    [JsonPropertyName("post_title")]
    public string PostTitle { get; set; } = string.Empty;

    [JsonPropertyName("post_status")]
    public string PostStatus { get; set; } = string.Empty;

    [JsonPropertyName("post_type")]
    public string PostType { get; set; } = string.Empty;
}