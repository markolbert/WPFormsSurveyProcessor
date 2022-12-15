using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class WpPostsParser
{
    private record FieldDefinition( string Type, string FieldText );

    private readonly IJ4JLogger _logger;

    public WpPostsParser(
        IJ4JLogger logger
    )
    {
        _logger = logger;
        _logger.SetLoggedType( GetType() );
    }

    public SurveyDownload? ParseFile( string filePath )
    {
        if( !File.Exists( filePath ) )
        {
            _logger.Error<string>("File '{0}' does not exist or is not accessible", filePath);
            return null;
        }

        var rawJson = JsonSerializer.Deserialize<JsonElement>( File.ReadAllText( filePath ) );

        if( rawJson.ValueKind != JsonValueKind.Array )
        {
            _logger.Error<string>("{0} did not parse to a JSON array", filePath);
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var retVal = new SurveyDownload();

        foreach (var element in rawJson.EnumerateArray())
        {
            var text = element.GetRawText();
            
            var objType = JsonSerializer.Deserialize<WpType>( text, options );
            if( objType == null )
            {
                _logger.Error("Could not determine object type for header object");
                return null;
            }

            var type = objType.Type.ToLower();
            var objText = element.GetRawText();

            switch ( type )
            {
                case "header":
                    retVal.Header = JsonSerializer.Deserialize<DownloadHeader>( objText, options );
                    break;

                case "database":
                    retVal.Database = JsonSerializer.Deserialize<DownloadDatabase>(objText, options);
                    break;

                case "table":
                    retVal.Table = JsonSerializer.Deserialize<DownloadTable>(objText, options);
                    break;

                default:
                    _logger.Warning<string>("Unexpected download header type '{0}' encountered", type);
                    break;
            }
        }

        if( !retVal.IsValid )
            _logger.Error("Survey download failed to parse completely");
        else ParseForms(retVal );

        return retVal;
    }

    private void ParseForms( SurveyDownload download )
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, 
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        foreach ( var surveyDef in download.Table!.Data! )
        {
            if( surveyDef.PostContent == null )
                continue;

            var postContent = JsonSerializer.Deserialize<SurveyPostContent>( surveyDef.PostContent, options );

            // for some strange reason, some WpForms forms have the Fields object as a JsonArray, and some 
            // have it as a JsonObject. We need to accomodate both
            var fieldDefEnumerator = postContent!.Fields.ValueKind switch
            {
                JsonValueKind.Array => EnumerateFieldsArray( postContent.Fields, options ),
                JsonValueKind.Object => EnumerateFieldsObject( postContent.Fields, options ),
                _ => UnsupportedEnumerator( postContent.Fields.ValueKind )
            };

            foreach( var fieldDef in fieldDefEnumerator )
            {
                var newField = fieldDef.Type switch
                {
                    "content" => JsonSerializer.Deserialize<ContentField>( fieldDef.FieldText, options ),
                    "name" => JsonSerializer.Deserialize<NameField>( fieldDef.FieldText, options ),
                    "email" => JsonSerializer.Deserialize<EmailField>( fieldDef.FieldText, options ),
                    "radio" => CreateRadioField( fieldDef.FieldText, options ),
                    "text" => JsonSerializer.Deserialize<TextField>( fieldDef.FieldText, options ),
                    "number-slider" => JsonSerializer.Deserialize<NumberSliderField>( fieldDef.FieldText, options ),
                    "textarea" => JsonSerializer.Deserialize<TextField>( fieldDef.FieldText, options ),
                    "checkbox" => CreateCheckboxField( fieldDef.FieldText, options ),
                    "phone" => JsonSerializer.Deserialize<PhoneField>(fieldDef.FieldText, options),
                    "html"=> JsonSerializer.Deserialize<HtmlField>(fieldDef.FieldText, options),
                    "divider" => JsonSerializer.Deserialize<DividerField>(fieldDef.FieldText, options),
                    "pagebreak"=> JsonSerializer.Deserialize<PageBreakField>(fieldDef.FieldText, options),
                    "likert_scale" => CreateLikertField(fieldDef.FieldText, options ),
                    "password" => JsonSerializer.Deserialize<PasswordField>(fieldDef.FieldText, options),
                    "select" => JsonSerializer.Deserialize<SelectField>(fieldDef.FieldText, options),
                    "file-upload" => JsonSerializer.Deserialize<FileUploadField>(fieldDef.FieldText, options),
                    _ => (FieldBase?) null
                };

                if( newField == null )
                    _logger.Error<string>( "Failed to parse survey field, type '{0}'", fieldDef.Type );
                else surveyDef.Fields.Add( newField );
            }
        }
    }

    private IEnumerable<FieldDefinition> EnumerateFieldsObject(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                _logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    private IEnumerable<FieldDefinition> EnumerateFieldsArray(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateArray())
        {
            var fieldText = fieldObj.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                _logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    private IEnumerable<FieldDefinition> UnsupportedEnumerator( JsonValueKind valueKind )
    {
        _logger.Error( "Unsupported fields ValueKind '{0}'", valueKind );
        yield break;
    }

    private CheckboxField? CreateCheckboxField( string text, JsonSerializerOptions options )
    {
        var retVal = JsonSerializer.Deserialize<CheckboxField>( text, options );

        return retVal;
    }

    private RadioField? CreateRadioField(string text, JsonSerializerOptions options)
    {
        var retVal = JsonSerializer.Deserialize<RadioField>(text, options);

        return retVal;
    }

    private LikertField? CreateLikertField( string text, JsonSerializerOptions options )
    {
        var retVal = JsonSerializer.Deserialize<LikertField>( text, options );

        return retVal;
    }
}
