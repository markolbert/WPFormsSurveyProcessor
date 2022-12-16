namespace WpFormsSurvey;

public record DownloadFormTable( string Name, string Database, List<FormDefinition>? Data )
{
    public List<FormInfo> Forms =>
        Data?.Select( x => new FormInfo( x.Id, x.PostTitle ) )
             .Distinct( FormInfo.DefaultComparer )
             .OrderBy( x => x.Id )
             .ToList()
     ?? new List<FormInfo>();
}