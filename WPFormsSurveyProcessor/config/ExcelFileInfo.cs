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

using J4JSoftware.Utilities;

namespace J4JSoftware.WpFormsSurvey;

public class ExcelFileInfo
{
    private string _fileName = "Survey Results.xlsx";
    private string _directory = Environment.CurrentDirectory;

    public Exporters InformationToExport { get; set; } = Exporters.All;
    public NamedRangeConfigurations RangeConfigurations { get; set; } = new();

    public string FileName
    {
        get => _fileName;

        set
        {
            var newName = Path.EndsInDirectorySeparator( value )
                ? $"{value}Survey Results.xlsx"
                : string.IsNullOrEmpty(value) 
                    ? "Survey Results.xlsx" 
                    : $"{Path.GetFileNameWithoutExtension(value)}.xlsx";

            CanBeWritten = FileExtensions.ValidateFilePath( newName, out var temp, ".xlsx", requireWriteAccess: true );

            if( string.IsNullOrEmpty( temp ) )
            {
                _fileName = newName;
                _directory = Environment.CurrentDirectory;
            }
            else
            {
                _fileName = Path.GetFileName( temp );

                var tempDir = Path.GetDirectoryName( temp );
                if( !string.IsNullOrEmpty( tempDir ) )
                    _directory = tempDir;
            }
        }
    }

    public string Directory
    {
        get => _directory;

        set
        {
            var filePath = Path.Combine( value, _fileName );
            CanBeWritten = FileExtensions.ValidateFilePath(filePath, out var temp, ".xlsx", requireWriteAccess: true);

            if( string.IsNullOrEmpty( temp ) )
            {
                _fileName = "Survey Results.xlsx";
                _directory = Environment.CurrentDirectory;
            }
            else
            {
                _fileName = Path.GetFileName( temp );
                _directory = Path.GetDirectoryName( temp ) ?? Environment.CurrentDirectory;
            }
        }
    }

    public string GetTimeStampedPath( string pathToEntriesFile )
    {
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

        return TimeStamp switch
        {
            ExcelTimeStamp.DateOnly => Path.Combine( _directory,
                                                     Path.GetFileNameWithoutExtension( _fileName )
                                                   + dtCreation.ToString( " yyyy-MM-dd" )
                                                   + ".xlsx" ),
            ExcelTimeStamp.DateAndTime => Path.Combine( _directory,
                                                        Path.GetFileNameWithoutExtension( _fileName )
                                                      + dtCreation.ToString( " yyyy-MM-dd hh-mm-ss" )
                                                      + ".xlsx" ),
            _ => Path.Combine( _directory, _fileName )
        };
    }

    public ExcelTimeStamp TimeStamp { get; set; } = ExcelTimeStamp.DateAndTime;
    public bool CanBeWritten { get; private set; }
}
