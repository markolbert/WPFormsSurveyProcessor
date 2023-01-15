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

using System.ComponentModel;
using J4JSoftware.DependencyInjection;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.WpFormsSurvey;

public class Configuration : IDisposable
{
    public const string DefaultConfigurationFile = "appConfig.json";
    public const string DefaultFileName = "Survey Results.xlsx";

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

    public string ExcelPath { get; set; } = DefaultFileName;
    public bool ExcelFileExists { get; private set; }
    public List<WorksheetInfo> WorksheetDefinitions { get; set; } = new();
    public Exporters InformationToExport { get; set; } = Exporters.All;
    public IWorkbook? Workbook { get; private set; }

    public List<int> FormIds { get; set; } = new();
    public bool DisplayFormInfo { get; set; }

    public bool ShowHelp { get; set; }
    public bool ShowDocumentation { get; set; }
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Information;

    public bool FinalizeConfiguration()
    {
        if( ShowHelp || ShowDocumentation )
            return true;

        var retVal = ConfirmConfigurationFile() & ValidateDataFile( PostsFilePath, "Posts" );

        if ( DisplayFormInfo )
            return retVal;

        retVal &= ValidateWorksheetDefinitions() && ValidateExcelFile();

        _logger.Information<string, string>( "{0} Excel file {1}",
                                             ExcelFileExists ? "Opening" : "Creating",
                                             ExcelPath );

        // NPOI has what strikes me as a weird interaction with streams
        // - if you load an existing spreadsheet from a stream, it closes the stream after reading
        // - if you load an existing spreadsheet from a file path, it >>doesn't<< close the stream after
        //   reading...and you can't access the stream it's created
        // Which then causes problems when you try to write back to the same file
        if( ExcelFileExists )
        {
            using var fs = File.OpenRead( ExcelPath );
            Workbook = new XSSFWorkbook( fs );
        }
        else Workbook = new XSSFWorkbook( XSSFWorkbookType.XLSX );

        return ValidateDataFile( EntriesFilePath, "Entries" ) && retVal;
    }

    private bool ConfirmConfigurationFile()
    {
        if( string.IsNullOrEmpty( ConfigurationFilePath ) )
        {
            _logger.Warning<string>( "Using default configuration file {0}", DefaultConfigurationFile );
            ConfigurationFilePath = DefaultConfigurationFile;
        }
        else _logger.Verbose<string>( "Using configuration file {0}", ConfigurationFilePath );

        var configLocator = new FileLocator( _logger )
                           .FileToFind( ConfigurationFilePath )
                           .Required()
                           .StopOnFirstMatch()
                           .ScanCurrentDirectory()
                           .ScanExecutableDirectory();

        if( configLocator.Matches == 1 )
            return true;

        _logger.Error( "Couldn't find app configuration file" );

        return false;
    }

    private bool ValidateDataFile(string filePath, string fileType)
    {
        var status = FileStatus.Undefined;

        if (!string.IsNullOrEmpty(filePath))
            status = File.Exists(filePath) ? FileStatus.Okay : FileStatus.Nonexistent;

        if (status != FileStatus.Okay)
            _logger.Error<string, string>("{0} file not {1}",
                                  fileType,
                                  status switch
                                  {
                                      FileStatus.Undefined => "specified",
                                      _ => "accessible"
                                  });

        return status == FileStatus.Okay;
    }

    private bool ValidateWorksheetDefinitions()
    {
        var retVal = true;

        // since we're doing an export, check for duplicate worksheet descriptions
        if (WorksheetDefinitions.Distinct(WorksheetInfo.SheetTypeComparer).Count() != WorksheetDefinitions.Count)
        {
            _logger.Error("WorksheetDefinitions configuration contains multiple descriptions for the same worksheet");
            retVal = false;
        }

        // ...and ensure all the worksheet descriptions are valid
        if (!WorksheetDefinitions.All(x => x.IsValid()))
        {
            _logger.Error("WorksheetDefinitions configuration contains one or more errors");
            retVal = false;
        }

        // ...and that we have descriptions for all the sheets we'll be creating
        foreach (var exporter in Exporters.All.GetUniqueFlags())
        {
            EnsureWorksheetDescription(exporter switch
            {
                Exporters.ChoiceFields => SheetType.Choices,
                Exporters.FieldDescriptions => SheetType.Fields,
                Exporters.FormInformation => SheetType.Forms,
                Exporters.Responses => SheetType.Responses,
                _ => throw new InvalidEnumArgumentException($"Unsupported Exporters value '{exporter}'")
            });
        }

        return retVal;
    }

    private bool ValidateExcelFile()
    {
        if (string.IsNullOrEmpty(ExcelPath))
        {
            _logger.Warning<string>("Writing to default Excel file '{0}'", DefaultFileName);
            ExcelPath = DefaultFileName;
        }

        var excelLocator = new FileLocator(_logger).FileToFind(ExcelPath)
                                                   .Writeable()
                                                   .StopOnFirstMatch()
                                                   .ScanCurrentDirectory()
                                                   .ScanExecutableDirectory();

        if (excelLocator.Matches != 1)
        {
            _logger.Error("User does not have write access to the directory where the Excel file will be written");
            return false;
        }

        var firstExcel = excelLocator.FirstMatch!;
        ExcelPath = firstExcel.Path;
        ExcelFileExists = firstExcel.State.HasFlag(PathState.Exists);
        
        return true;
    }

    private bool SheetsExist( List<string> sheetNames, string[] needNames, int suffixNum )
    {
        var suffix = suffixNum <= 1 ? string.Empty : suffixNum.ToString();

        return sheetNames
           .Any( x => needNames.Any( y => y.Equals( $"{x}{suffix}", StringComparison.OrdinalIgnoreCase ) ) );
    }

    private void EnsureWorksheetDescription( SheetType required )
    {
        if( WorksheetDefinitions.Any( x => x.SheetType == required ) )
            return;

        WorksheetDefinitions.Add( new WorksheetInfo() { SheetName = required.ToString().ToLower(), SheetType = required } );
        _logger.Warning( "Added default worksheet description for {0}", required );
    }

    public void Dispose()
    {
        Workbook?.Close();
        Workbook?.Dispose();
    }
}