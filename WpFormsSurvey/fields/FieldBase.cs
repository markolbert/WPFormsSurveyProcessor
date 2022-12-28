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
public class FieldBase : JsonField
{
    protected FieldBase()
    {
    }

    public Form? Form { get; private set; }

    public virtual bool Initialize( Form formDef )
    {
        Form = formDef;
        return true;
    }

    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string FieldType { get; set; } = string.Empty;

    [JsonConverter(typeof(WpFormsBooleanConverter))]
    public bool SurveyField { get; set; }

    [JsonPropertyName("conditional_logic")]
    public int ConditionalLogic { get; set; }

    [JsonPropertyName("conditional_type")]
    public string ConditionalType { get; set; } = string.Empty;

    public List<List<FieldConditional>>? Conditionals { get; set; }
}