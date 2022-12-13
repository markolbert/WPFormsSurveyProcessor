using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class FormDefinitionConverter : JsonConverter<List<FieldBase>>
{
    private readonly IJ4JLogger _logger;

    public FormDefinitionConverter(
        IJ4JLogger logger
    )
    {
        _logger = logger;
        _logger.SetLoggedType( GetType() );
    }

    public override List<FieldBase>? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        dynamic formData = new ExpandoObject();
        var curData = formData;

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            _logger.Error("JSON reader not starting with StartObject");
            throw new JsonException();
        }

        while (reader.Read())
        {
            if( curData is not IDictionary<string, object?> dictionary )
            {
                _logger.Error("Could not convert the current ExpandoObject to an IDictionary<string, object?>");
                throw new JsonException();
            }

            switch( reader.TokenType )
            {
                case JsonTokenType.StartObject:
                    dynamic childObject = new ExpandoObject();
                    childObject.Parent = curData;

                    if (!dictionary.ContainsKey("Children"))
                        curData.Children = new List<ExpandoObject>();

                    curData.Children.Add(childObject);

                    curData = childObject;

                    break;

                case JsonTokenType.EndObject:
                    if( dictionary.ContainsKey( "Parent" ) )
                        curData = curData.Parent;
                    else return CreateFormDefinition( formData );

                    break;

                case JsonTokenType.PropertyName:
                    dynamic propObject = new ExpandoObject();
                    propObject.Name = reader.GetString() ?? string.Empty;
                    propObject.Parent = curData;

                    if( !dictionary.ContainsKey( "Properties" ) )
                        curData.Properties = new List<ExpandoObject>();

                    curData.Properties.Add( propObject );
                    curData = propObject;

                    break;

                case JsonTokenType.StartArray:
                    dynamic listObject = new List<ExpandoObject>();
                    listObject.Parent = curData;

                    dictionary.Add( "Array", listObject );

                    curData = listObject;

                    break;

                case JsonTokenType.EndArray:
                    if( !dictionary.ContainsKey( "Parent" ) )
                    {
                        _logger.Error( "No parent to return to from array" );
                        throw new JsonException();
                    }

                    curData = curData.Parent;

                    break;

                case JsonTokenType.Comment:
                    reader.Skip();
                    break;

                case JsonTokenType.False:
                case JsonTokenType.True:
                    curData.Value = reader.GetBoolean();
                    break;

                case JsonTokenType.Number:
                    curData.Value = reader.GetDecimal();
                    break;

                case JsonTokenType.String:
                    curData.Value = reader.GetString() ?? string.Empty;
                    break;

                case JsonTokenType.None:
                    curData.Value = "***none***";
                    break;

                case JsonTokenType.Null:
                    curData.Value = "***null***";
                    break;
            }
        }

        throw new JsonException();
    }

    private List<FieldBase> CreateFormDefinition( ExpandoObject formData )
    {
    }

    public override void Write( Utf8JsonWriter writer, List<FieldBase> value, JsonSerializerOptions options )
    {
        _logger.Fatal("Converting to JSON is not supported");

        throw new NotImplementedException();
    }
}
