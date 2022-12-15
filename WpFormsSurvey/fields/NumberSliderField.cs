namespace WPFormsSurvey;

[JsonFieldName("number-slider")]
public class NumberSliderField : TextField
{
    public int Minimum { get; set; }
    public int Maximum { get; set; }
    public int Step { get; set; }
}
