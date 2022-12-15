namespace WpFormsSurvey;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class JsonFieldNameAttribute : Attribute
{
    public JsonFieldNameAttribute(
        string entityName
    )
    {
        EntityName = entityName;
    }

    public string EntityName { get; }
}
