﻿using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using WpFormsSurvey;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class ResponseDefinition
{
    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }

    [JsonPropertyName("form_id")]
    public int FormId { get; set; }

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyName("date_modified")]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public JsonElement Fields { get; set; }

    [JsonIgnore]
    public List<ResponseBase> Responses { get; set; } = new();
}