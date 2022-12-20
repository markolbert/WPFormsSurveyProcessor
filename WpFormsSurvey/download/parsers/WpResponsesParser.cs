using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;

namespace WpFormsSurvey;

public record IndividualResponse( int FormId, int FieldId, ResponseBase Response );

public class WpResponsesParser : WpParserBase<ResponseBase>
{
    public WpResponsesParser(
        IJ4JLogger logger
    )
        : base( logger )
    {
    }

    public Responses? ParseFile( string filePath )
    {
        if( !File.Exists( filePath ) )
        {
            Logger.Error<string>( "File '{0}' does not exist or is not accessible", filePath );
            return null;
        }

        var rawJson = JsonSerializer.Deserialize<JsonElement>( File.ReadAllText( filePath ) );

        if( rawJson.ValueKind != JsonValueKind.Array )
        {
            Logger.Error<string>( "{0} did not parse to a JSON array", filePath );
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var retVal = new Responses();

        foreach( var element in rawJson.EnumerateArray() )
        {
            var text = element.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>( text, options );
            if( objType == null )
            {
                Logger.Error( "Could not determine object type for header object" );
                return null;
            }

            var type = objType.Type.ToLower();
            var objText = element.GetRawText();

            switch( type )
            {
                case "header":
                    retVal.Header = JsonSerializer.Deserialize<Header>( objText, options );
                    break;

                case "database":
                    retVal.Database = JsonSerializer.Deserialize<Database>( objText, options );
                    break;

                case "table":
                    retVal.Table = JsonSerializer.Deserialize<Entries>( objText, options );
                    break;

                default:
                    Logger.Warning<string>( "Unexpected download header type '{0}' encountered", type );
                    break;
            }
        }

        if( !retVal.IsValid )
            Logger.Error( "Forms response download failed to parse completely" );
        else ParseResponses( retVal );

        return retVal;
    }

    private void ParseResponses( Responses download )
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, 
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        foreach( var responseDef in download.Table!.Data! )
        {
            if( string.IsNullOrEmpty( responseDef.Fields ) )
                continue;

            var fieldsElement = JsonSerializer.Deserialize<JsonElement>( responseDef.Fields, options );

            foreach ( var responseObj in fieldsElement.EnumerateObject() )
            {
                var responseText = responseObj.Value.GetRawText();

                var respType = JsonSerializer.Deserialize<WpType>(responseText, options);
                if( respType == null )
                {
                    Logger.Error("Could not parse WpType"  );
                    continue;
                }

                if ( !EntityTypes.ContainsKey( respType.Type ) )
                {
                    Logger.Warning<string>( "No response type is registered for key '{0}'", respType.Type );
                    continue;
                }

                var newResponse = (ResponseBase?) EntityTypes[ respType.Type ]
                                                 .DeserializerInfo
                                                 .Invoke( null, new object[] { responseText, options } );

                if( newResponse == null )
                    Logger.Error<string>( "Failed to parse survey response, type '{0}'", responseText );
                else
                {
                    if( newResponse.Initialize() )
                        responseDef.Responses.Add( newResponse );
                    else
                        Logger.Error( "Response for field {0} on form {1} for user Id {2} failed to initialize",
                                      newResponse.FieldId,
                                      responseDef.FormId,
                                      responseDef.UserId );
                }
            }
        }
    }
}
