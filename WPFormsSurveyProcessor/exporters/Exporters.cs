namespace J4JSoftware.WpFormsSurvey;

[Flags]
public enum Exporters
{
    FormInformation = 1 << 0,
    FieldDescriptions = 1 << 1,
    ChoiceFields = 1 << 2,
    Submissions = 1 << 3,

    All = FormInformation | FieldDescriptions | ChoiceFields | Submissions
}
