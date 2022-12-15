namespace WpFormsSurvey;

public class ResponseDownload
{
    public DownloadHeader? Header { get; set; }
    public DownloadDatabase? Database { get; set; }
    public DownloadResponseTable? Table { get; set; }

    public bool IsValid => Header != null && Database != null && Table is { Data: { } };
}
