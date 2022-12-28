using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("content")]
public class ContentResponse : ResponseBase
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

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
                                                                 Content ) );

        return retVal;
    }
}
