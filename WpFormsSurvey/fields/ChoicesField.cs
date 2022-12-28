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

[WpFormsFieldType("select")]
[WpFormsFieldType("radio")]
public class ChoicesField : LabeledField
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    [JsonPropertyName("choices")]
    public JsonElement RawChoices { get; set; }

    [JsonIgnore]
    public List<FieldChoice> Choices { get; } = new();

    public override bool Initialize( Form formDef )
    {
        if( !base.Initialize( formDef ) )
            return false;

        // for some strange reason, some WpForms forms have the Fields object as a JsonArray, and some 
        // have it as a JsonObject. We need to accomodate both
        var fieldDefEnumerator = RawChoices.ValueKind switch
        {
            JsonValueKind.Array => EnumerateArray<FieldChoice>(RawChoices, _options),
            JsonValueKind.Object => EnumerateObject<FieldChoice>( RawChoices, _options ),
            _ => UnsupportedEnumerator<FieldChoice>(RawChoices.ValueKind)
        };

        var retVal = true;

        foreach( var choice in fieldDefEnumerator )
        {
            if( choice == null )
                retVal = false;
            else Choices.Add( choice );
        }

        return retVal;
    }
}
