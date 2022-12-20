using System.Runtime.CompilerServices;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace WPFormsSurveyProcessor;

internal partial class DeusEx : J4JDeusExHosted
{
    protected override J4JHostConfiguration? GetHostConfiguration()
    {
        var hostConfig = new J4JHostConfiguration( AppEnvironment.Console )
                        .ApplicationName( "WpFormsSurveyProcessor" )
                        .Publisher( "Jump for Joy Software" )
                        .LoggerInitializer( ConfigureLogging )
                        .AddDependencyInjectionInitializers( ConfigureDependencyInjection )
                        .FilePathTrimmer( FilePathTrimmer );

        var cmdLineConfig = hostConfig.AddCommandLineProcessing( CommandLineOperatingSystems.Windows )
                                      .OptionsInitializer( CommandLineConfiguration );

        return hostConfig;
    }

    private void ConfigureLogging(
        IConfiguration config,
        J4JHostConfiguration hostConfig,
        J4JLoggerConfiguration loggerConfig
    )
    {
        loggerConfig.SerilogConfiguration
                    .WriteTo.Debug()
                    .WriteTo.Console();
    }

    private static string FilePathTrimmer(Type? loggedType,
        string callerName,
        int lineNum,
        string srcFilePath)
    {
        return CallingContextEnricher.DefaultFilePathTrimmer(loggedType,
                                                             callerName,
                                                             lineNum,
                                                             CallingContextEnricher.RemoveProjectPath(srcFilePath,
                                                                 GetProjectPath()));
    }

    private static string GetProjectPath([CallerFilePath] string filePath = "")
    {
        // DirectoryInfo will throw an exception when this method is called on a machine
        // other than the development machine, so just return an empty string in that case
        try
        {
            var dirInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePath)!);

            while (dirInfo.Parent != null)
            {
                if (dirInfo.EnumerateFiles("*.csproj").Any())
                    break;

                dirInfo = dirInfo.Parent;
            }

            return dirInfo.FullName;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
