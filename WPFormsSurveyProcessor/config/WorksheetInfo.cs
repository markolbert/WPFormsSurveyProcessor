using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace J4JSoftware.WpFormsSurvey;

public class WorksheetInfo
{
    private sealed class SheetTypeEqualityComparer : IEqualityComparer<WorksheetInfo>
    {
        public bool Equals( WorksheetInfo? x, WorksheetInfo? y )
        {
            if( ReferenceEquals( x, y ) )
                return true;
            if( ReferenceEquals( x, null ) )
                return false;
            if( ReferenceEquals( y, null ) )
                return false;
            if( x.GetType() != y.GetType() )
                return false;

            return x.SheetType == y.SheetType;
        }

        public int GetHashCode( WorksheetInfo obj )
        {
            return (int) obj.SheetType;
        }
    }

    public static IEqualityComparer<WorksheetInfo> SheetTypeComparer { get; } = new SheetTypeEqualityComparer();

    private readonly IJ4JLogger _logger;

    public WorksheetInfo()
    {
        _logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();
        _logger.SetLoggedType(GetType());
    }

    public SheetType SheetType { get; set; } = SheetType.Undefined;
    public string SheetName { get; set; } = string.Empty;
    public List<NamedRangeConfiguration> Ranges { get; set; } = new();

    public bool HasRanges => Ranges.Any();

    public bool IsValid()
    {
        var retVal = true;

        if (string.IsNullOrEmpty(SheetName))
        {
            _logger.Error("SheetName is undefined");
            retVal = false;
        }

        if( HasRanges && !Ranges.All( x => x.IsValid() ) )
        {
            _logger.Error<string>("Sheet '{0}' has one or more invalid named ranges", SheetName  );
            retVal = false;
        }

        if( SheetType != SheetType.Undefined )
            return retVal;

        _logger.Error<string>("Sheet '{0}' is an unsupported sheet type", SheetName);
        retVal = false;

        return retVal;
    }
}
