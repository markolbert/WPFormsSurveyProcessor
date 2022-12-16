namespace WpFormsSurvey;

public record DownloadResponseTable( string Name, string Database, List<ResponseDefinition>? Data )
{
    public List<int> FormIds =>
        Data?.Select( x => x.FormId )
             .Distinct()
             .OrderBy( x => x )
             .ToList()
     ?? new List<int>();
}
