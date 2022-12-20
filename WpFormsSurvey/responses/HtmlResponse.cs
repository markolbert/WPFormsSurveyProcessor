using System.Text.Json.Serialization;

namespace WpFormsSurvey;

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
