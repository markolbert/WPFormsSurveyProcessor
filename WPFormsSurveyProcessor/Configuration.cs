using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.DeusEx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WPFormsSurveyProcessor;

public class Configuration
{
    public enum FileStatus
    {
        Nonexistent,
        Undefined,
        Okay
    }

    private string _entriesFilePath = string.Empty;
    private string _postsFilePath = string.Empty;

    public string PostsFilePath
    {
        get => _postsFilePath;

        set
        {
            _postsFilePath = value;
            PostsFileStatus = Status( _postsFilePath );
        }
    }

    public FileStatus PostsFileStatus { get; private set; } = FileStatus.Undefined;

    public string EntriesFilePath
    {
        get => _entriesFilePath;

        set
        {
            _entriesFilePath = value;
            EntriesFileStatus = Status( _entriesFilePath );
        }
    }

    public FileStatus EntriesFileStatus { get; private set; } = FileStatus.Undefined;

    public List<int> FormIds { get; set; } = new();
    public bool DisplayFormInfo { get; set; }
    public bool ShowHelp { get; set; }

    public IHostedService ServiceToRun
    {
        get
        {
            if( Errors != null )
                return J4JDeusEx.ServiceProvider.GetRequiredService<MisconfigurationService>();

            if ( DisplayFormInfo )
                return J4JDeusEx.ServiceProvider.GetRequiredService<FormInfoService>();

            if( ShowHelp)
                return J4JDeusEx.ServiceProvider.GetRequiredService<HelpService>();

            return J4JDeusEx.ServiceProvider.GetRequiredService<ParseService>();
        }
    }

    public List<string>? Errors
    {
        get
        {
            if( ShowHelp )
                return null;

            if( DisplayFormInfo && EntriesFileStatus != FileStatus.Okay && PostsFileStatus != FileStatus.Okay )
                return new List<string>
                {
                    EntriesFileStatus switch
                    {
                        FileStatus.Undefined => "Entries file not specified",
                        _ => "Entries file not accessible"
                    },
                    PostsFileStatus switch
                    {
                        FileStatus.Undefined => "Posts file not specified",
                        _ => "Posts file not accessible"
                    }
                };

            if( EntriesFileStatus == FileStatus.Okay && PostsFileStatus != FileStatus.Okay )
                return new List<string>
                {
                    PostsFileStatus switch
                    {
                        FileStatus.Undefined => "Posts file not specified",
                        _ => "Posts file not accessible"
                    }
                };

            if( EntriesFileStatus != FileStatus.Okay && PostsFileStatus != FileStatus.Okay )
                return new List<string> { "Posts and entries files not specified" };

            return null;
        }
    }

    private FileStatus Status( string filePath )
    {
        if (string.IsNullOrEmpty(filePath))
            return FileStatus.Undefined;

        return !File.Exists(PostsFilePath)
            ? FileStatus.Nonexistent
            : FileStatus.Okay;
    }
}
