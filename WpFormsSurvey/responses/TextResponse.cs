namespace WpFormsSurvey;

[WpFormsFieldType("text")]
[WpFormsFieldType("textarea")]
[WpFormsFieldType("radio")]
[WpFormsFieldType("select")]
[WpFormsFieldType("phone")]
[WpFormsFieldType("email")]
[WpFormsFieldType("password")]
[WpFormsFieldType("file-upload")]
public class TextResponse : ResponseBase
{
    public string Value { get; set; } = string.Empty;
}