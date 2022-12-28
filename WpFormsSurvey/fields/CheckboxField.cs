using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("checkbox")]
public class CheckboxField : ChoicesField
{
    [JsonPropertyName("choice_limit")]
    [JsonConverter(typeof(WpFormsIntegerConverter))]
    public int ChoiceLimit { get; set; }
}