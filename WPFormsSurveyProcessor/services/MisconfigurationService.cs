using J4JSoftware.Logging;
using Serilog.Events;

namespace WPFormsSurveyProcessor;

internal class MisconfigurationService : ServiceBase
{
    public MisconfigurationService(
        Configuration config,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        if( Configuration.Errors == null )
            return Task.CompletedTask;

        foreach (var error in Configuration.Errors)
        {
            Logger.Write(LogEventLevel.Fatal, error);
        }

        Environment.ExitCode = 1;

        return Task.CompletedTask;
    }
}
