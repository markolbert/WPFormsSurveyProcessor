namespace WPFormsSurvey;

public class RadioField : TextField
{
    public List<FieldChoice> Choices { get; set; } = new();
}
