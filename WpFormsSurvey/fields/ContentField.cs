namespace WPFormsSurvey;

public class ContentField : FieldBase
{
    internal ContentField( Dictionary<string, object?> fieldValues )
        : base( fieldValues )
    {
        Content = fieldValues.ContainsKey( "content" ) ? (string) fieldValues[ "content" ]! : string.Empty;
    }

    public string Content { get; }
}
