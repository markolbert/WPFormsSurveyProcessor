// Copyright (c) 2022 Mark A. Olbert 
// all rights reserved
// This file is part of WpFormsSurvey.
//
// WpFormsSurvey is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// WpFormsSurvey is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with WpFormsSurvey. If not, see <https://www.gnu.org/licenses/>.

using System.Text.Json.Serialization;

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