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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("likert_scale")]
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

    public override bool Initialize( Form formDef )
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
