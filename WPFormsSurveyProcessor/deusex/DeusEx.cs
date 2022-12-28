// Copyright (c) 2022 Mark A. Olbert 
// all rights reserved
// This file is part of WpFormsSurveyProcessor.
//
// WpFormsSurveyProcessor is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// WpFormsSurveyProcessor is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with WpFormsSurveyProcessor. If not, see <https://www.gnu.org/licenses/>.

using System.Runtime.CompilerServices;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace J4JSoftware.WpFormsSurvey;

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
                                      .OptionsInitializer( SetCommandLineConfiguration )
                                      .ConfigurationFileKeys( true, false, "c", "config" );

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
