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

[WpFormsFieldType("html")]
public class HtmlResponse : ResponseBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    public override ResponseExport GetResponseExport(
        Form form,
        IndividualSubmission submission,
        int fieldId
    )
    {
        var retVal = base.GetResponseExport( form, submission, fieldId );

        if( retVal.IsValid )
            retVal.Responses.Add( new UserFieldResponse<string>( submission.UserId,
                                                                 submission.FormId,
                                                                 submission.IpAddress,
                                                                 submission.Date,
                                                                 fieldId,
                                                                 retVal.Field!.FieldType,
                                                                 Code ) );
        return retVal;
    }
}
