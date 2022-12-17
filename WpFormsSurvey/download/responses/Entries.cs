namespace WpFormsSurvey;

public record Entries( string Name, string Database, List<IndividualSubmission>? Data )
{
    public List<int> FormIds =>
        Data?.Select( x => x.FormId )
             .Distinct()
             .OrderBy( x => x )
             .ToList()
     ?? new List<int>();
}
