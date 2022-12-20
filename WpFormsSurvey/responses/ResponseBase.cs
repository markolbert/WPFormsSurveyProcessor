using System.Text.Json.Serialization;

namespace WpFormsSurvey;

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