namespace WPFormsSurvey;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JsonFieldNameAttribute : Attribute
{
    public JsonFieldNameAttribute(
        string fieldName
    )
    {
        FieldName = fieldName;
    }

    public string FieldName { get; }
}
