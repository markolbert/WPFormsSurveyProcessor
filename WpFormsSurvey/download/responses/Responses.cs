namespace WpFormsSurvey;

public class Responses
{
    public Header? Header { get; set; }
    public Database? Database { get; set; }
    public Entries? Table { get; set; }

    public bool IsValid => Header != null && Database != null && Table is { Data: { } };
}
