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

[WpFormsFieldType("checkbox")]
[WpFormsFieldType("radio")]
[WpFormsFieldType("select")]
public class MultipleTextResponse : ResponseBase
{
    private string _rawValue = string.Empty;

    [ JsonPropertyName( "value" ) ]
    public string RawValue
    {
        get => _rawValue;

        set
        {
            _rawValue = value;

            Values.Clear();
            Values.AddRange( _rawValue.Split( "\n" ).Select( x => x.Trim() ) );
        }
    }

    [JsonIgnore]
    public List<string> Values { get; } = new();

    public override ResponseExport GetResponseExport(
        Form form,
        IndividualSubmission submission,
        int fieldId
    )
    {
        var retVal = base.GetResponseExport(form, submission, fieldId);

        if (!retVal.IsValid)
            return retVal;

        if (retVal.Field is not ChoicesField choicesField)
        {
            retVal.Error =
                $"Field associated with user {submission.UserId} response for field {FieldId} in form {form.PostTitle} (id {form.Id}) is not a ChoicesField";
            return retVal;
        }

        foreach( var response in Values )
        {
            var responseInfo = new IndexedResponseInfo( response,
                                                        choicesField.Choices.FindIndex( x => x.Label.Equals( response,
                                                            StringComparison.OrdinalIgnoreCase ) ) );

            retVal.Responses.Add( new UserFieldResponse<IndexedResponseInfo>( submission.UserId,
                                                                         submission.FormId,
                                                                         submission.IpAddress,
                                                                         submission.Date,
                                                                         FieldId,
                                                                         choicesField.FieldType,
                                                                         responseInfo ) );
        }

        return retVal;
    }
}
