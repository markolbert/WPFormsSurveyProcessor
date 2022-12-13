namespace WPFormsSurvey;

public class NameField : TextField
{
    internal NameField(Dictionary<string, object?> fieldValues)
        : base(fieldValues)
    {
        Format = fieldValues.ContainsKey("format") ? (string)fieldValues["format"]! : string.Empty;
    }

    public string Format { get; }
}
