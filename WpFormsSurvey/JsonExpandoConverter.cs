using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class JsonExpandoConverter<TTarget> : JsonConverter<TTarget>
{
    private readonly IJsonDynamicCreator<TTarget> _creator;
    private readonly IJ4JLogger _logger;

    public JsonExpandoConverter(
        IJsonDynamicCreator<TTarget> creator,
        IJ4JLogger logger
    )
    {
        _creator = creator;

        _logger = logger;
        _logger.SetLoggedType( GetType() );
    }

    public override TTarget? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        dynamic formData = new ExpandoObject();
        dynamic curData = formData;

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            _logger.Error("JSON reader not starting with StartObject");
            throw new JsonException();
        }

        while (reader.Read())
        {
            switch( reader.TokenType )
            {
                case JsonTokenType.StartObject:
                    dynamic childObject = new ExpandoObject();
                    childObject.Parent = curData;

                    if (!ExpandoUtils.HasElement(curData, "Children"))
                        curData.Children = new List<ExpandoObject>();

                    curData.Children.Add(childObject);

                    curData = childObject;

                    break;

                case JsonTokenType.EndObject:
                    if( !ExpandoUtils.HasElement( curData, "Parent" ) )
                        curData = curData.Parent;
                    else return _creator.Create( formData );

                    break;

                case JsonTokenType.PropertyName:
                    dynamic propObject = new ExpandoObject();
                    propObject.Name = reader.GetString() ?? string.Empty;
                    propObject.Parent = curData;

                    if( !ExpandoUtils.HasElement(curData, "Properties" ) )
                        curData.Properties = new List<ExpandoObject>();

                    curData.Properties.Add( propObject );
                    curData = propObject;

                    break;

                case JsonTokenType.StartArray:
                    dynamic listObject = new List<ExpandoObject>();
                    listObject.Parent = curData;

                    curData.Array = listObject;

                    curData = listObject;

                    break;

                case JsonTokenType.EndArray:
                    if( !ExpandoUtils.HasElement(curData, "Parent" ) )
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

    public override void Write( Utf8JsonWriter writer, TTarget value, JsonSerializerOptions options )
    {
        _logger.Fatal("Converting to JSON is not supported");

        throw new NotImplementedException();
    }
}
