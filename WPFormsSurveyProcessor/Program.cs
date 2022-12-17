using System;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.Configuration.J4JCommandLine;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace WPFormsSurveyProcessor;

internal class Program
{
    static void Main( string[] args )
    {
        var deusEx = new DeusEx();

        if( !deusEx.Initialize() )
        {
            J4JDeusEx.Logger?.Fatal("Could not initialize application");
            Environment.ExitCode = 1;
            return;
        }

        //var logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();

        //Configuration = J4JDeusEx.ServiceProvider.GetRequiredService<Configuration>();
        //if( Configuration.Errors != null )
        //{
        //    foreach( var error in Configuration.Errors )
        //    {
        //        logger.Write( LogEventLevel.Fatal, error );
        //    }

        //    Environment.ExitCode = 1;
        //    return;
        //}

        //if ( Configuration.ShowHelp )
        //{
        //    var options = J4JDeusEx.ServiceProvider.GetRequiredService<OptionCollection>();
        //    var help = new ColorHelpDisplay( new WindowsLexicalElements( logger ), options );
        //    help.Display();

        //    Environment.ExitCode = 1;
        //    return;
        //}

        var config = J4JDeusEx.ServiceProvider.GetRequiredService<Configuration>();
        var cancellationTokenSrc = new CancellationTokenSource();
        config.ServiceToRun.StartAsync( cancellationTokenSrc.Token );
    }
}