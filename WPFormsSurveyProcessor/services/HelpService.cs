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

namespace WPFormsSurveyProcessor;

internal class HelpService : ServiceBase
{
    private readonly OptionCollection _options;

    public HelpService(
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

        return Task.CompletedTask;
    }
}
