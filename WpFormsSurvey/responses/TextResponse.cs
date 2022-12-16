namespace WpFormsSurvey;

[WpFormsFieldType("text")]
[WpFormsFieldType("textarea")]
[WpFormsFieldType("radio")]
[WpFormsFieldType("phone")]
[WpFormsFieldType("email")]
public class TextResponse : ResponseBase
{
    public string Value { get; set; } = string.Empty;
}