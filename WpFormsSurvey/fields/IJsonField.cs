namespace WPFormsSurvey;

public interface IJsonField
{
    public bool IsValid { get; }
    public bool Initialize();
}
