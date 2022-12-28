namespace J4JSoftware.WpFormsSurvey;

public class FormsDownload
{
    public Header? Header { get; set; } 
    public Database? Database { get; set; }
    public Forms? Table { get; set; }

    public bool IsValid => Header != null && Database != null && Table is { Data: {} };
}