namespace WPFormsSurveyProcessor;

public class NamedRangeConfiguration
{
    private string? _lastCol;

    public string Name { get; set; } = string.Empty;
    public NamedRangeContext Context { get; set; } = NamedRangeContext.Workbook;
    public string FirstColumn { get; set; } = string.Empty;
    public bool IncludeHeader { get; set; }

    public string LastColumn
    {
        get => string.IsNullOrEmpty(_lastCol) ? FirstColumn : _lastCol;
        set => _lastCol = value;
    }

    public bool IsValid => !Errors.Any();

    public List<string> Errors
    {
        get
        {
            var retVal = new List<string>();

            if( string.IsNullOrEmpty( FirstColumn ) )
                retVal.Add( "First column is empty or undefined" );

            if( string.IsNullOrEmpty( LastColumn ) )
                retVal.Add( "Last column is empty or undefined" );

            if( FirstColumn.Any( x => !char.IsLetter( x ) ) )
                retVal.Add( "First column contains invalid characters" );

            if( LastColumn.Any( x => !char.IsLetter( x ) ) )
                retVal.Add( "Last column contains invalid characters" );

            var firstIndex = FirstColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );
            var lastIndex = LastColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );

            if( firstIndex > lastIndex )
                retVal.Add( "Last column precedes first column" );

            return retVal;
        }
    }
}