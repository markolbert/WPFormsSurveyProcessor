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
public abstract class ResponseBase : JsonField
{
    protected ResponseBase()
    {
    }

    [JsonPropertyName("id")]
    public int FieldId { get; set; }

    [JsonPropertyName("name")]
    public string FieldLabel { get; set; } = string.Empty;

    public virtual bool Initialize() => true;

    public virtual ResponseExport GetResponseExport(
        Form form,
        IndividualSubmission submission,
        int fieldId
    )
    {
        var retVal = new ResponseExport { Field = form.Fields.FirstOrDefault( x => x.Id == fieldId ) };

        if( retVal.Field == null )
            retVal.Error =
                $"Could not process user {submission.UserId} response for field {FieldId} in form {form.PostTitle} (id {form.Id})";

        return retVal;
    }
}