﻿using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class Form
{
    public int Id { get; set; }

    [JsonPropertyName("post_author")]
    public string PostAuthor { get; set; } = string.Empty;

    [JsonConverter(typeof(WpDateTimeConverter))]
    [JsonPropertyName("post_date")]
    public DateTime PostDate { get; set; }

    [JsonConverter(typeof(WpDateTimeConverter))]
    [JsonPropertyName("post_date_gmt")]
    public DateTime PostDateGmt { get; set; }

    [JsonPropertyName("post_title")]
    public string PostTitle { get; set; } = string.Empty;

    [JsonPropertyName("post_status")]
    public string PostStatus { get; set; } = string.Empty;

    [JsonPropertyName("post_type")]
    public string? PostType { get; set; }

    [JsonPropertyName("post_content")]
    public string? PostContent { get; set; }

    [JsonIgnore]
    public List<FieldBase> Fields { get; set; } = new();

    public bool HasSurveyFields =>
        Fields.Any( x => x.SurveyField )
     || HasFields<ChoicesField>();

    public bool HasFields<TField>()
        where TField : FieldBase =>
        Fields.Any( x => x.GetType().IsAssignableTo( typeof( TField ) ) );
}