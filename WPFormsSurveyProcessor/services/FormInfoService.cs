using J4JSoftware.Logging;

namespace WPFormsSurveyProcessor;

internal class FormInfoService : ServiceBase
{
    public FormInfoService(
        Configuration config,
        IJ4JLogger logger
    )
        : base( config, logger )
    {
    }
}
