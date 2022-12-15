using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

public class WpFormsIntegerConverter : JsonConverter<int>
{
    public override int Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) =>
        int.TryParse( reader.GetString(), out var temp ) ? temp : 0;

    public override void Write( Utf8JsonWriter writer, int value, JsonSerializerOptions options ) =>
        writer.WriteStringValue( value.ToString() );
}