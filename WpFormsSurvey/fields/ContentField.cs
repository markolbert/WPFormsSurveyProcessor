namespace WpFormsSurvey;

[JsonFieldName("content")]
public class ContentField : FieldBase
{
    public string Content { get; set; } = string.Empty;
}
