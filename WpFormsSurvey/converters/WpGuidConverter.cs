using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;
public class WpGuidConverter : JsonConverter<Guid>
{
    public override Guid Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        var text = reader.GetString();

        return Guid.TryParse(text, out var guidValue) ? guidValue : Guid.Empty;
    }

    public override void Write( Utf8JsonWriter writer, Guid value, JsonSerializerOptions options )
    {
        writer.WriteStringValue( value );
    }
}