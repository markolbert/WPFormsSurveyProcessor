namespace WpFormsSurvey;

public record NameResponseInfo( string FirstName, string MiddleName, string LastName )
{
    public NameResponseInfo(
        NameResponse response
    )
        : this( response.First, response.Middle, response.Last )
    {
    }
}
