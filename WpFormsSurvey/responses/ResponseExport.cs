namespace J4JSoftware.WpFormsSurvey;

public class ResponseExport
{
    public FieldBase? Field { get; set; }
    public List<IUserFieldResponse> Responses { get; } = new();
    public string? Error { get; set; }

    public bool IsValid => string.IsNullOrEmpty( Error );
}
