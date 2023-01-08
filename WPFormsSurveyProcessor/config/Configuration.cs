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

using J4JSoftware.DependencyInjection;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.WpFormsSurvey;

public class Configuration
{
    public const string DefaultConfigurationFile = "appConfig.json";

    private enum FileStatus
    {
        Nonexistent,
        Undefined,
        Okay
    }

    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new() { MinimumLevel = LogEventLevel.Warning };

    private readonly IJ4JLogger _logger;

    public Configuration()
    {
        _logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();
        _logger.SetLoggedType(GetType());

        _logger.Verbose("Initialized logger in Configuration");
    }

    public string ConfigurationFilePath { get; set; } = DefaultConfigurationFile;
    public string PostsFilePath { get; set; } = string.Empty;
    public string EntriesFilePath { get; set; } = string.Empty;

    public ExcelFileInfo ExcelFileInfo { get; set; } = new();

    public List<int> FormIds { get; set; } = new();
    public bool DisplayFormInfo { get; set; }

    public bool ShowHelp { get; set; }
    public bool ShowDocumentation { get; set; }
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Warning;

    public bool IsValid()
    {
        if( ShowHelp || ShowDocumentation )
            return true;

        var retVal = true;

        if (string.IsNullOrEmpty(ConfigurationFilePath))
        {
            _logger.Warning<string>("Using default configuration file {0}", DefaultConfigurationFile);
            ConfigurationFilePath = DefaultConfigurationFile;
        }
        else _logger.Verbose<string>("Using configuration file {0}", ConfigurationFilePath);

        if( !FileExtensions.ValidateFilePath( ConfigurationFilePath,
                                              out var result,
                                              folders: FileFolders.Default,
                                              logger: _logger ) )
        {
            _logger.Error<string>( "Configuration file path '{0}' is invalid or inaccessible",
                                   ConfigurationFilePath );
            retVal = false;
        }

        if( !Status(PostsFilePath, out var temp  ) )
        {
            _logger.Error<string>( "Posts file not {0}",
                                   temp!.Value switch
                                   {
                                       FileStatus.Undefined => "specified",
                                       _ => "accessible"
                                   } );

            retVal = false;
        }

        if( DisplayFormInfo )
            return retVal;

        if( Status(EntriesFilePath, out var temp2  ) )
            return retVal;

        _logger.Error<string>( "Entries file not {0}",
                               temp2!.Value switch
                               {
                                   FileStatus.Undefined => "specified",
                                   _ => "accessible"
                               } );

        return false;
    }

    private bool Status( string filePath, out FileStatus? result )
    {
        result = FileStatus.Undefined;

        if( !string.IsNullOrEmpty( filePath ) )
            result = File.Exists( filePath ) ? FileStatus.Okay : FileStatus.Nonexistent;

        return result == FileStatus.Okay;
    }
}