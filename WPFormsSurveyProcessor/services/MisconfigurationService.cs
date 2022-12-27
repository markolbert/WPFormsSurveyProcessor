using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.Configuration.J4JCommandLine;
using J4JSoftware.Logging;
using Serilog.Events;

namespace WPFormsSurveyProcessor;

internal class MisconfigurationService : ServiceBase
{
    private readonly OptionCollection _options;

    public MisconfigurationService(
        Configuration config,
        OptionCollection options,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
        _options = options;
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        var help = new ColorHelpDisplay(new WindowsLexicalElements(Logger), _options);
        help.Display();

        if ( Configuration.Errors == null )
            return Task.CompletedTask;

        foreach (var error in Configuration.Errors)
        {
            Logger.Write(LogEventLevel.Fatal, error);
        }

        Environment.ExitCode = 1;

        return Task.CompletedTask;
    }
}
