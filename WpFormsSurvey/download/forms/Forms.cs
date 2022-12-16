namespace WpFormsSurvey;

public record Forms( string Name, string Database, List<Form>? Data )
{
    public List<FormInfo> SummaryInfo =>
        Data?.Select( x => new FormInfo( x.Id, x.PostTitle ) )
             .Distinct( FormInfo.DefaultComparer )
             .OrderBy( x => x.Id )
             .ToList()
     ?? new List<FormInfo>();
}