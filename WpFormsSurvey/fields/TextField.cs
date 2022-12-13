namespace WPFormsSurvey;

public class TextField : FieldBase
{
    internal TextField(Dictionary<string, object?> fieldValues)
        : base(fieldValues)
    {
        Label = fieldValues.ContainsKey("label") ? (string)fieldValues["label"]! : string.Empty;
    }

    public string Label { get; }
}