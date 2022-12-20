namespace WpFormsSurvey;

[WpFormsFieldType("number-slider")]
public class NumericResponse : ResponseBase
{
    public decimal Value { get; set; }

    public override ResponseExport GetResponseExport(
        Form form,
        IndividualSubmission submission,
        int fieldId
    )
    {
        var retVal = base.GetResponseExport(form, submission, fieldId);

        if( retVal.IsValid )
            retVal.Responses.Add( new UserFieldResponse<decimal>( submission.UserId,
                                                                  submission.FormId,
                                                                  submission.IpAddress,
                                                                  submission.Date,
                                                                  fieldId,
                                                                  retVal.Field!.FieldType,
                                                                  Value ) );
        return retVal;
    }
}
