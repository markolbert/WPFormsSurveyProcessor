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

namespace J4JSoftware.WpFormsSurvey;

public class ExcelFileInfo
{
    public const string DefaultFileName = "Survey Results.xlsx";

    private readonly IJ4JLogger _logger;

    public ExcelFileInfo()
    {
        _logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();
        _logger.SetLoggedType( GetType() );

        _logger.Verbose( "Initialized logger in ExcelFileInfo" );
    }

    public Exporters InformationToExport { get; set; } = Exporters.All;
    public NamedRangeConfigurations RangeConfigurations { get; set; } = new();

    public string ExcelPath { get; set; } = DefaultFileName;

    public bool GetTimeStampedPath( string pathToEntriesFile, out string? result )
    {
        result = null;
        if( !IsValid() )
            return false;

        var dtCreation = DateTime.Now;

        try
        {
            var entriesFileInfo = new FileInfo( pathToEntriesFile );
            dtCreation = entriesFileInfo.CreationTime;
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
        }

        result= TimeStamp switch
        {
            ExcelTimeStamp.DateOnly => System.IO.Path.Combine( Path.GetDirectoryName(ExcelPath) ?? Environment.CurrentDirectory,
                                                               Path.GetFileNameWithoutExtension(ExcelPath)
                                                             + dtCreation.ToString( " yyyy-MM-dd" )
                                                             + ".xlsx" ),
            ExcelTimeStamp.DateAndTime => System.IO.Path.Combine( Path.GetDirectoryName(ExcelPath) ?? Environment.CurrentDirectory,
                                                                  Path.GetFileNameWithoutExtension( ExcelPath )
                                                                + dtCreation.ToString( " yyyy-MM-dd hh-mm-ss" )
                                                                + ".xlsx" ),
            _ => ExcelPath
        };

        return true;
    }

    public ExcelTimeStamp TimeStamp { get; set; } = ExcelTimeStamp.DateAndTime;

    public bool IsValid()
    {
        if( System.IO.Path.EndsInDirectorySeparator( ExcelPath ) )
        {
            ExcelPath = $"{ExcelPath}{DefaultFileName}";
            _logger.Information<string>("Added {0} to Excel file path", DefaultFileName);
        }
        else
        {
            if( string.IsNullOrEmpty( ExcelPath ) )
            {
                ExcelPath = DefaultFileName;
                _logger.Information<string>("Changed Excel file name to {0}", DefaultFileName);
            }
        }

        return FileExtensions.ValidateFilePath(ExcelPath,
                                                  out var temp,
                                                  ".xlsx",
                                                  requireWriteAccess: true,
                                                  logger: _logger);
    }

}
