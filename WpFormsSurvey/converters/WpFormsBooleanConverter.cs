using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

public class WpFormsBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();

        if (int.TryParse(text, out var temp))
            return temp == 1;

        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "1" : "0");
    }
}