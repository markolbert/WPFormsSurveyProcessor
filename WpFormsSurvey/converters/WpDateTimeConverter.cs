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
public class WpDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        var text = reader.GetString();

        return DateTime.TryParse(text, out var dtValue) ? dtValue : DateTime.MinValue;
    }

    public override void Write( Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options )
    {
        writer.WriteStringValue( value );
    }
}