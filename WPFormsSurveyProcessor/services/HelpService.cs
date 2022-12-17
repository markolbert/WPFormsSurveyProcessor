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

internal class HelpService : IHostedService
{
    public Task StartAsync( CancellationToken cancellationToken )
    {
        var options = J4JDeusEx.ServiceProvider.GetRequiredService<OptionCollection>();
        var logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();

        var help = new ColorHelpDisplay(new WindowsLexicalElements(logger), options);
        
        help.Display();

        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}
