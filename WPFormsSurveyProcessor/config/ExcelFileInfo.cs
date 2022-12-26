using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Utilities;

namespace WPFormsSurveyProcessor;

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
