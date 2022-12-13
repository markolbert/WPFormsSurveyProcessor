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

                    if (!HasElement(curData, "Children"))
                        curData.Children = new List<ExpandoObject>();

                    curData.Children.Add(childObject);

                    curData = childObject;

                    break;

                case JsonTokenType.EndObject:
                    if( !HasElement( curData, "Parent" ) )
                        curData = curData.Parent;
                    else return CreateFormDefinition( formData );

                    break;

                case JsonTokenType.PropertyName:
                    dynamic propObject = new ExpandoObject();
                    propObject.Name = reader.GetString() ?? string.Empty;
                    propObject.Parent = curData;

                    if( !HasElement(curData, "Properties" ) )
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
                    if( !HasElement(curData, "Parent" ) )
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

    private bool HasElement(ExpandoObject toCheck, string name) => ((IDictionary<string, object?>)toCheck).ContainsKey(name);

    private List<FieldBase> CreateFormDefinition( ExpandoObject formData )
    {
        var retVal = new List<FieldBase>();

        if( !GetExpandoObject(formData,"Children", out var temp) )
            return retVal;

        dynamic children = temp!;

        foreach (KeyValuePair<string, ExpandoObject> kvp in children)
        {
            if( CreateField(kvp.Value, out var newField))
                retVal.Add(newField!);
        }

        return retVal;
    }

    private bool GetExpandoObject(ExpandoObject container, string name, out ExpandoObject? result)
    {
        result = null;

        if (((IDictionary<string, object?>)container).TryGetValue(name, out var temp))
        {
            if (temp is ExpandoObject temp2)
            {
                result = temp2;
                return true;
            }

            _logger.Error<string>("Element '{0}' is not an ExpandoObject", name);
        }
        else _logger.Error("FormData object does not contain a Children property");

        return false;
    }

    private bool CreateField(ExpandoObject fieldData, out FieldBase? result)
    {
        result = null;

        if( !GetPropertyValue<string>(fieldData, "type", out var fieldType ))
            return false;

        if (string.IsNullOrEmpty(fieldType))
            return false;

        result = fieldType.ToLower() switch
        {
            "content" => CreateContentField(fieldData),
            _ => null
        };

        return result != null;
    }

    private bool GetPropertyValue<TProp>(ExpandoObject container, string name, out TProp? result )
    {
        result = default(TProp);

        if (((IDictionary<string, object?>)container).TryGetValue(name, out var temp))
        {
            if (temp is TProp temp2)
            {
                result = temp2;
                return true;
            }

            _logger.Error<string, Type>("Property '{0}' is not a {1}", name, typeof(TProp));
        }
        else _logger.Error<string>("ExpandoObject does not contain '{0}'", name);

        return false;
    }

    private ContentField CreateContentField(ExpandoObject container)
    {
        var retVal = InitializeField<ContentField>(container);

        retVal.Content = GetPropertyValue<string>(container, "content", out var tempContent)
            ? tempContent!
            : string.Empty;

        return retVal;
    }

    private TField InitializeField<TField>(ExpandoObject container )
        where TField : FieldBase, new()
    {
        var retVal = new TField()
        {
            Id = GetPropertyValue<int>(container, "id", out var tempId) ? tempId : 0
        };

        return retVal;
    }

    public override void Write( Utf8JsonWriter writer, List<FieldBase> value, JsonSerializerOptions options )
    {
        _logger.Fatal("Converting to JSON is not supported");

        throw new NotImplementedException();
    }
}
