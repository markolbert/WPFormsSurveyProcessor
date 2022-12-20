using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("checkbox")]
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
            var responseInfo = new IndexedResponseInfo( FieldLabel,
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
