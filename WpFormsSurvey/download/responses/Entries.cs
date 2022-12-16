namespace WpFormsSurvey;

public record Entries( string Name, string Database, List<Entry>? Data )
{
    public List<int> FormIds =>
        Data?.Select( x => x.FormId )
             .Distinct()
             .OrderBy( x => x )
             .ToList()
     ?? new List<int>();
}
