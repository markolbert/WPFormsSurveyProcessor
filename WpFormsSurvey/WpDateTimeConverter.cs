using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;
public class WpDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        var text = reader.GetString();

        return DateTime.TryParse(text, out var dtValue) ? dtValue : DateTime.MinValue;
    }

    public override void Write( Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options )
    {
        writer.WriteStringValue( value );
    }
}