namespace WPFormsSurveyProcessor;

public record NamedRangeConfiguration(string Name, string FirstColumn, bool IncludeHeader)
{
    private string? _lastCol;

    public string LastColumn
    {
        get => string.IsNullOrEmpty(_lastCol) ? FirstColumn : _lastCol;
        init => _lastCol = value;
    }
}