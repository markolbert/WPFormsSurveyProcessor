using System.Diagnostics;
using J4JSoftware.Logging;

namespace WPFormsSurveyProcessor;

internal class DocumentationService : ServiceBase
{
    public DocumentationService( 
        Configuration config, 
        IJ4JLogger logger 
    )
        : base( config, logger )
    {
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        var docsDir = Path.Combine( Environment.CurrentDirectory, "docs" );

        var htmlPath = Path.Combine(docsDir, "docs.html");
        if( !File.Exists( htmlPath ) )
        {
            Logger.Warning("Documentation file not available or accessible");
            return Task.CompletedTask;
        }

        var htmlProcess = new Process { StartInfo = new ProcessStartInfo( htmlPath ) { UseShellExecute = true } };
        htmlProcess.Start();

        return Task.CompletedTask;
    }
}
