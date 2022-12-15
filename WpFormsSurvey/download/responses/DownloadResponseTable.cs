namespace WpFormsSurvey;

public record DownloadResponseTable(string Name, string Database, List<ResponseDefinition>? Data);
