using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("text")]
[WpFormsFieldType("textarea")]
[WpFormsFieldType("phone")]
[WpFormsFieldType("email")]
[WpFormsFieldType("password")]
[WpFormsFieldType("file-upload")]
public class TextResponse : ResponseBase
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    public override ResponseExport GetResponseExport(
        Form form,
        IndividualSubmission submission,
        int fieldId
    )
    {
        var retVal = base.GetResponseExport(form, submission, fieldId);

        if (retVal.IsValid)
            retVal.Responses.Add(new UserFieldResponse<string>(submission.UserId,
                                                               submission.FormId,
                                                               submission.IpAddress,
                                                               submission.Date,
                                                               fieldId,
                                                               retVal.Field!.FieldType,
                                                               Value));
        return retVal;
    }
}
