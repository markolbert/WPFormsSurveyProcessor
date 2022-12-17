using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.Configuration.J4JCommandLine;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
