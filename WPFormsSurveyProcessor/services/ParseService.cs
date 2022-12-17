using J4JSoftware.Logging;

namespace WPFormsSurveyProcessor;

internal class ParseService : ServiceBase
{
    public ParseService(
        Configuration config,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
    }
}
