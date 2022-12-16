using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class ResponseBase 
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string FieldLabel { get; set; } = string.Empty;

    public FormDefinition? Form => Field?.Form;
    public FieldBase? Field { get; private set; }

    public virtual bool Initialize( FieldBase field )
    {
        Field = field;

        return field.Form != null;
    }
}
