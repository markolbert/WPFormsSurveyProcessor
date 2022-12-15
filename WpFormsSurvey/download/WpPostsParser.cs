using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class WpPostsParser
{
    private record FieldDefinition( string Type, string FieldText );
    
    private record FieldInfo( Type FieldType, MethodInfo DeserializerInfo );

    private readonly Dictionary<string, FieldInfo> _fieldTypes = new(StringComparer.OrdinalIgnoreCase);
    private readonly MethodInfo? _genericDeserializer;
    private readonly IJ4JLogger _logger;

    public WpPostsParser(
        IJ4JLogger logger
    )
    {
        var thisType = GetType();

        _logger = logger;
        _logger.SetLoggedType( thisType );

        var temp = typeof(JsonSerializer)
                           .GetMethods()
                           .FirstOrDefault(x =>
                            {
                                if (!x.IsGenericMethod || x.GetGenericArguments().Length != 1)
                                    return false;

                                var args = x.GetParameters();

                                if (args.Length != 2)
                                    return false;

                                if (args[0].ParameterType != typeof(string))
                                    return false;

                                return args[1].ParameterType == typeof(JsonSerializerOptions);
                            });

        if( temp == null )
            throw new ApplicationException( $"Could not find {nameof( JsonSerializer.Deserialize )}" );
        
        _genericDeserializer = temp;

        RegisterFieldTypes(thisType.Assembly);
    }

    public bool RegisterFieldType<TField>() => RegisterFieldType( typeof( TField ) );

    public bool RegisterFieldType( Type fieldType )
    {
        if( typeof( IJsonField ).IsAssignableFrom( fieldType ) )
            return RegisterFieldInternal( fieldType );

        _logger.Error("{0} does not implement IJsonField", fieldType);
        return false;
    }

    public bool RegisterFieldTypes( Assembly assembly )
    {
        var retVal = true;

        foreach( var type in assembly.GetTypes().Where( x => typeof( IJsonField ).IsAssignableFrom( x ) ) )
        {
            retVal &= RegisterFieldInternal( type );
        }

        return retVal;
    }

    private bool RegisterFieldInternal( Type fieldType )
    {
        var attr = fieldType.GetCustomAttribute<JsonFieldNameAttribute>(false);
        if (attr == null)
        {
            _logger.Error("{0} is not decorated with a JsonFieldNameAttribute", fieldType);
            return false;
        }

        if( fieldType.GetConstructor( Type.EmptyTypes ) == null )
        {
            _logger.Error("{0} does not have a public parameterless constructor", fieldType);
            return false;
        }

        var fieldInfo = new FieldInfo(fieldType, _genericDeserializer!.MakeGenericMethod(fieldType));

        if (_fieldTypes.ContainsKey(attr.FieldName))
        {
            _fieldTypes[attr.FieldName] = fieldInfo;
            _logger.Information<string>("Replaced IJsonField for field '{0}'", attr.FieldName);
        }
        else _fieldTypes.Add(attr.FieldName, fieldInfo);

        return true;
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

        foreach( var surveyDef in download.Table!.Data! )
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
                if( !_fieldTypes.ContainsKey( fieldDef.Type ) )
                {
                    _logger.Warning<string>("No field type is registered for key '{0}'", fieldDef.Type);
                    continue;
                }

                var newField = (FieldBase?) _fieldTypes[ fieldDef.Type ]
                                       .DeserializerInfo.Invoke( null, new object[] { fieldDef.FieldText, options } );

                if( newField == null )
                    _logger.Error<string>("Failed to parse survey field, type '{0}'", fieldDef.Type);
                else
                {
                    if( !newField.Initialize() )
                        _logger.Error<string>("{0} field failed to initialize", fieldDef.Type);
                    else surveyDef.Fields.Add(newField);
                }
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
}
