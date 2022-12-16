using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;

namespace WpFormsSurvey;

[WpFormsFieldType("likert-scale")]
public class LikertField : TextField
{
    [JsonPropertyName("single_row")]
    [JsonConverter(typeof(WpFormsBooleanConverter))]
    public bool SingleRow { get; set; }

    [JsonPropertyName("rows")]
    public JsonElement RawRows { get; set; }

    [JsonIgnore]
    public List<string> Rows { get; } = new();

    [JsonPropertyName("columns")]
    public JsonElement RawColumns { get; set; }

    [JsonIgnore]
    public List<string> Columns { get; } = new();

    public override bool Initialize( FormDefinition formDef )
    {
        if( !base.Initialize(formDef))
            return false;

        // WpForms stores rows and columns as JsonObjects with each property corresponding
        // to a row or column. However, the property names are not valid under C#
        // (they're the row or column index)
        Rows.Clear();
        Columns.Clear();

        foreach (var row in RawRows.EnumerateObject())
        {
            Rows.Add(row.Value.GetString() ?? string.Empty);
        }

        foreach (var column in RawColumns.EnumerateObject())
        {
            Columns.Add(column.Value.GetString() ?? string.Empty);
        }

        return true;
    }

}
