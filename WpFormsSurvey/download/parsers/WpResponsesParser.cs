using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace WpFormsSurvey;

public class WpResponsesParser : WpParserBase<ResponseBase>
{
    public WpResponsesParser(
        IJ4JLogger logger
    )
        : base( logger )
    {
        RegisterEntityTypes( GetType().Assembly );
    }

    public Responses? ParseFile( List<Form> forms, string filePath )
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
        else ParseResponses( retVal, forms );

        return retVal;
    }

    private void ParseResponses( Responses download, List<Form> forms )
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, 
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        foreach( var responseDef in download.Table!.Data! )
        {
            // find the form we're processing responses for
            var form = forms.FirstOrDefault( x => x.Id == responseDef.FormId );
            if( form == null )
            {
                Logger.Error( "Could not find form with Id {0}", responseDef.FormId );
                continue;
            }

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
                    // find the field we're a response to
                    var field = form.Fields.FirstOrDefault( x => x.Id == newResponse.Id );
                    if( field == null )
                        Logger.Error<int, string>( "Could not find field with Id {0} in form '{1}'",
                                                   newResponse.Id,
                                                   form.PostTitle );
                    else
                    {
                        if( !newResponse.Initialize( field ) )
                            Logger.Error<string>( "{0} response failed to initialize", respType.Type );
                        else responseDef.Responses.Add( newResponse );
                    }
                }
            }
        }
    }
}
