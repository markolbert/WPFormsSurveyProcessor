using J4JSoftware.Logging;
using Microsoft.Extensions.Hosting;

namespace J4JSoftware.WpFormsSurvey;

internal class ServiceBase : IHostedService
{
    protected ServiceBase(
        Configuration config,
        IJ4JLogger logger
    )
    {
        Configuration = config;

        Logger = logger;
        Logger.SetLoggedType( GetType() );
    }

    protected Configuration Configuration { get; }
    protected IJ4JLogger Logger { get;}

    public virtual Task StartAsync( CancellationToken cancellationToken )
    {
        Logger.Warning("{0} not implemented", GetType());

        return Task.CompletedTask;
    }

    public virtual Task StopAsync( CancellationToken cancellationToken )
    {
        return Task.CompletedTask;
    }
}
