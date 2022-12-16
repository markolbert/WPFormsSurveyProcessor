namespace WpFormsSurvey;

[WpFormsFieldType("name")]
public class NameResponse : ResponseBase
{
    public string First { get; set; } = string.Empty;
    public string Middle { get; set; } = string.Empty;
    public string Last { get; set; } = string.Empty;
}
