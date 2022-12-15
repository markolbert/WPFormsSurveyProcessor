using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class LikertField : TextField
{
    [JsonPropertyName("rows")]
    public JsonElement RawRows { get; set; }

    [JsonIgnore]
    public List<string> Rows { get; } = new();

    [JsonPropertyName("columns")]
    public JsonElement RawColumns { get; set; }

    [JsonIgnore]
    public List<string> Columns { get; } = new();
}
