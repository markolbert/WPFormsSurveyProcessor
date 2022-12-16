using J4JSoftware.Logging;

namespace WpFormsSurvey;

[WpFormsFieldType("text")]
[WpFormsFieldType("textarea")]
[WpFormsFieldType("email")]
[WpFormsFieldType("divider")]
[WpFormsFieldType("password")]
public class TextField : FieldBase
{
    public string Label { get; set; } = string.Empty;
}