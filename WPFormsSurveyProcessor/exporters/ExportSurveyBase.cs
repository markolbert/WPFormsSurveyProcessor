using J4JSoftware.Logging;

namespace WPFormsSurveyProcessor;

internal abstract class ExportSurveyBase<TEntity> : ExportBase<TEntity>
    where TEntity : class
{
    protected ExportSurveyBase( 
        Configuration config, 
        IJ4JLogger logger, 
        int reportingInterval = 500 
        )
        : base( logger, reportingInterval )
    {
        Configuration = config;
    }

    protected Configuration Configuration { get; }
    protected NamedRangeConfigurations RangeConfigurations => Configuration.ExcelFileInfo.RangeConfigurations;
}
