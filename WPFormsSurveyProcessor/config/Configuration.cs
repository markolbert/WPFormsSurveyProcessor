﻿// Copyright (c) 2022 Mark A. Olbert 
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

public class Configuration
{
    public const string DefaultConfigurationFile = "appConfig.json";

    public enum FileStatus
    {
        Nonexistent,
        Undefined,
        Okay
    }

    private string _entriesFilePath = string.Empty;
    private string _postsFilePath = string.Empty;
    private string _configFilePath = "";
    private bool _configFilePathIsValid;

    public string ConfigurationFilePath
    {
        get => _configFilePath;

        set
        {
            if (string.IsNullOrEmpty(value))
                value = DefaultConfigurationFile;

            _configFilePathIsValid = FileExtensions.ValidateFilePath(value, out var result);
            _configFilePath = _configFilePathIsValid ? result! : value;
        }
    }

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

    public ExcelFileInfo ExcelFileInfo { get; set; } = new();

    public List<int> FormIds { get; set; } = new();
    public bool DisplayFormInfo { get; set; }
    public bool ShowHelp { get; set; }
    public bool ShowDocumentation { get; set; }

    public List<string>? Errors
    {
        get
        {
            if( ShowHelp || ShowDocumentation )
                return null;

            if (!_configFilePathIsValid)
                return new List<string> { "Configuration file path '{0}' is invalid or inaccessible", ConfigurationFilePath };

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