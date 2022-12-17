namespace WpFormsSurvey;

public class Responses
{
    public Header? Header { get; set; }
    public Database? Database { get; set; }
    public Entries? Table { get; set; }
    public List<IndividualResponse> IndividualResponses { get; } = new();

    public bool IsValid => Header != null && Database != null && Table is { Data: { } };
}
