using J4JSoftware.DeusEx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        var service = GetService();

        var cancellationTokenSrc = new CancellationTokenSource();
        service.StartAsync( cancellationTokenSrc.Token );
    }

    private static IHostedService GetService()
    {
        var config = J4JDeusEx.ServiceProvider.GetRequiredService<Configuration>();

        if (config.Errors != null)
            return J4JDeusEx.ServiceProvider.GetRequiredService<MisconfigurationService>();

        if (config.DisplayFormInfo)
            return J4JDeusEx.ServiceProvider.GetRequiredService<FormInfoService>();

        if (config.ShowHelp)
            return J4JDeusEx.ServiceProvider.GetRequiredService<HelpService>();

        return J4JDeusEx.ServiceProvider.GetRequiredService<ParseService>();
    }
}