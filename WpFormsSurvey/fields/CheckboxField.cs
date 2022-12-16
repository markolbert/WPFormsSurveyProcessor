using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("checkbox")]
public class CheckboxField : ChoicesField
{
    [JsonPropertyName("choice_limit")]
    [JsonConverter(typeof(WpFormsIntegerConverter))]
    public int ChoiceLimit { get; set; }
}