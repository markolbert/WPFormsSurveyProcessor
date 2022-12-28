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

namespace J4JSoftware.WpFormsSurvey;

public class JsonField
{
    protected JsonField()
    {
    }

    protected IEnumerable<TTarget?> EnumerateObject<TTarget>(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            yield return JsonSerializer.Deserialize<TTarget>(fieldText, options);
        }
    }

    protected IEnumerable<TTarget?> EnumerateArray<TTarget>(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateArray())
        {
            var fieldText = fieldObj.GetRawText();

            yield return JsonSerializer.Deserialize<TTarget>(fieldText, options);
        }
    }

    protected IEnumerable<TTarget> UnsupportedEnumerator<TTarget>(JsonValueKind valueKind)
    {
        yield break;
    }
}
