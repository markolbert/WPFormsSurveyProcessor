using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("likert_scale")]
public class LikertResponse : ResponseBase
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true, 
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private string _responseText = string.Empty;

    [ JsonPropertyName( "value" ) ]
    public string ResponseText
    {
        get => _responseText;

        set
        {
            _responseText = value;

            Responses.Clear();
            Responses.AddRange(_responseText.Split("\n").Select(x => x.Trim()));
        }
    }

    [JsonIgnore]
    public List<string> Responses { get; } = new();

    [JsonPropertyName("value_raw")]
    public JsonElement RawValue { get; set; }

    [JsonIgnore]
    public List<LikertScore> Scores { get; } = new();

    public override bool Initialize()
    {
        Scores.Clear();

        if( !base.Initialize() )
            return false;

        var idx = 1;

        foreach( var rowResponse in RawValue.EnumerateObject() )
        {
            Scores.Add(new LikertScore(idx, rowResponse.Value.GetInt32())  );
            idx++;
        }

        return true;
    }

    public override ResponseExport GetResponseExport( Form form, IndividualSubmission submission, int fieldId )
    {
        var retVal = base.GetResponseExport( form, submission, fieldId );

        if( !retVal.IsValid )
            return retVal;

        if( retVal.Field is not LikertField likertField )
        {
            retVal.Error =
                $"Field associated with user {submission.UserId} response for field {FieldId} in form {form.PostTitle} (id {form.Id}) is not a LikertField";
            return retVal;
        }

        foreach( var score in Scores )
        {
            var responseInfo = new LikertResponseInfo( score.Row,
                                                       likertField.Columns[ score.Column - 1 ],
                                                       score.Column );

            retVal.Responses.Add( new UserFieldResponse<LikertResponseInfo>( submission.UserId,
                                                                             submission.FormId,
                                                                             submission.IpAddress,
                                                                             submission.Date,
                                                                             FieldId,
                                                                             retVal.Field.FieldType,
                                                                             responseInfo ) );
        }

        return retVal;
    }
}
