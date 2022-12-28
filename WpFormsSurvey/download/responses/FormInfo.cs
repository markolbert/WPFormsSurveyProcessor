namespace J4JSoftware.WpFormsSurvey;

public record FormInfo( int Id, string Name )
{
    private sealed class IdEqualityComparer : IEqualityComparer<FormInfo>
    {
        public bool Equals( FormInfo? x, FormInfo? y )
        {
            if( ReferenceEquals( x, y ) )
                return true;
            if( ReferenceEquals( x, null ) )
                return false;
            if( ReferenceEquals( y, null ) )
                return false;
            if( x.GetType() != y.GetType() )
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode( FormInfo obj )
        {
            return obj.Id;
        }
    }

    public static IEqualityComparer<FormInfo> DefaultComparer { get; } = new IdEqualityComparer();
}
