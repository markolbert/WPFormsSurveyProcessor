using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class NameField : FormattedField
{
}

public class PhoneField : FormattedField
{
}

public class HtmlField : FieldBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
