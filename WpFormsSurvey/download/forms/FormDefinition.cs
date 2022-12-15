using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using WpFormsSurvey;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class FormDefinition
{
    public int Id { get; set; }

    [JsonPropertyName("post_author")]
    public string PostAuthor { get; set; } = string.Empty;

    [JsonConverter(typeof(DateTimeConverter))]
    [JsonPropertyName("post_date")]
    public DateTime PostDate { get; set; }

    [JsonConverter(typeof(DateTimeConverter))]
    [JsonPropertyName("post_date_gmt")]
    public DateTime PostDateGmt { get; set; }

    [JsonPropertyName("post_title")]
    public string PostTitle { get; set; } = string.Empty;

    [JsonPropertyName("post_status")]
    public string PostStatus { get; set; } = string.Empty;

    [JsonPropertyName("post_content")]
    public string? PostContent { get; set; }

    [JsonIgnore]
    public List<FieldBase> Fields { get; set; } = new();

    public bool HasSurveyFields =>
        Fields.Any( x => x.SurveyField )
     || HasFields<RadioField>()
     || HasFields<CheckboxField>()
     || HasFields<SelectField>();

    public bool HasFields<TField>()
        where TField : FieldBase =>
        Fields.Any( x => x.GetType().IsAssignableTo( typeof( TField ) ) );
}