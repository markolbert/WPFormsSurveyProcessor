using System.Runtime.CompilerServices;
using Autofac;
using J4JSoftware.Logging;
using Serilog;

namespace Test.Parsing;

public class TestBase
{
    private readonly ContainerBuilder _builder = new ContainerBuilder();

    protected TestBase()
    {
        Container = Configure();

        Logger = Container.Resolve<IJ4JLogger>();
        Logger.SetLoggedType(GetType());
    }
    protected IContainer Container { get; }
    protected IJ4JLogger Logger { get; }

    private IContainer Configure()
    {
        ConfigureContainer(_builder);

        return _builder.Build();
    }

    protected virtual void ConfigureContainer(ContainerBuilder builder)
    {
        builder.Register(c =>
            {
                var loggerConfig = new J4JLoggerConfiguration(FilePathTrimmer);

                loggerConfig.SerilogConfiguration
                    .WriteTo.Debug(outputTemplate: loggerConfig.GetOutputTemplate(true));

                return loggerConfig.CreateLogger();
            })
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    // these next two methods serve to strip the project path off of source code
    // file paths
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
        var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

        while (dirInfo.Parent != null)
        {
            if (dirInfo.EnumerateFiles("*.csproj").Any())
                break;

            dirInfo = dirInfo.Parent;
        }

        return dirInfo.FullName;
    }
}