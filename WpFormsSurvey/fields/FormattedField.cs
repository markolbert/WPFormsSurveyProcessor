namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("phone")]
[WpFormsFieldType("name")]
public class FormattedField : TextField
{
    public string Format { get; set; } = string.Empty;
}
