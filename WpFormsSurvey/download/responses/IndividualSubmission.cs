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
public class IndividualSubmission
{
    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }

    [JsonPropertyName("form_id")]
    public int FormId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("user_uuid")]
    [JsonConverter(typeof(WpGuidConverter))]
    public Guid UserGuid { get; set; }
        
    [JsonConverter(typeof(WpDateTimeConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyName("date_modified")]
    [JsonConverter(typeof(WpDateTimeConverter))]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public string? Fields { get; set; }

    [JsonIgnore]
    public List<ResponseBase> Responses { get; set; } = new();
}
