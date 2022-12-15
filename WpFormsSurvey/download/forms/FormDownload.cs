namespace WpFormsSurvey;

public class FormDownload
{
    public DownloadHeader? Header { get; set; } 
    public DownloadDatabase? Database { get; set; }
    public DownloadFormTable? Table { get; set; }

    public bool IsValid => Header != null && Database != null && Table is { Data: {} };
}