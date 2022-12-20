namespace WpFormsSurvey;

[WpFormsFieldType("name")]
public class NameResponse : ResponseBase
{
    public string First { get; set; } = string.Empty;
    public string Middle { get; set; } = string.Empty;
    public string Last { get; set; } = string.Empty;

    public override ResponseExport GetResponseExport( Form form, IndividualSubmission submission, int fieldId )
    {
        var retVal = base.GetResponseExport(form, submission, fieldId);

        if( retVal.IsValid )
            retVal.Responses.Add( new UserFieldResponse<NameResponseInfo>( submission.UserId,
                                                                           submission.FormId,
                                                                           submission.IpAddress,
                                                                           submission.Date,
                                                                           fieldId,
                                                                           retVal.Field!.FieldType,
                                                                           new NameResponseInfo(
                                                                               First,
                                                                               Middle,
                                                                               Last ) ) );
        return retVal;
    }
}
