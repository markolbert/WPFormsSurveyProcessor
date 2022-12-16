namespace WpFormsSurvey;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class WpFormsFieldTypeAttribute : Attribute
{
    public WpFormsFieldTypeAttribute(
        string entityName
    )
    {
        EntityName = entityName;
    }

    public string EntityName { get; }
}
