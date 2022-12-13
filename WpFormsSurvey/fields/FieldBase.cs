using System.Text.Json;
using J4JSoftware.Logging;

namespace WPFormsSurvey;

public class FieldBase
{
    public static FieldBase? Create( Dictionary<string, object?> fieldValues, IJ4JLogger logger )
    {
        if (!fieldValues.ContainsKey("id"))
        {
            logger.Error("JSON field object does not contain an ID property");
            throw new JsonException();
        }

        if (!fieldValues.ContainsKey("type"))
        {
            logger.Error("JSON field object does not contain a type property");
            throw new JsonException();
        }

        var newField = fieldValues[ "type" ]!.ToString()!.ToLower() switch
        {
            "content" => new ContentField(fieldValues),
            "name" => new NameField(fieldValues),
            "email" => new EmailField(fieldValues),
            _ => (FieldBase?) null
        };

        return newField;
    }

    protected FieldBase( Dictionary<string, object? > fieldValues )
    {
        Id = fieldValues.ContainsKey("id") ? (int)fieldValues["id"]! : -1;
    }

    public int Id { get; }
    public List<FieldConditional> Conditionals { get; set; } = new();
}
