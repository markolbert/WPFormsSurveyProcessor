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

using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.Configuration.J4JCommandLine;
using J4JSoftware.Logging;
using Serilog.Events;

namespace J4JSoftware.WpFormsSurvey;

internal class MisconfigurationService : ServiceBase
{
    private readonly OptionCollection _options;

    public MisconfigurationService(
        Configuration config,
        OptionCollection options,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
        _options = options;
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        var help = new ColorHelpDisplay(new WindowsLexicalElements(Logger), _options);
        help.Display();

        if ( Configuration.Errors == null )
            return Task.CompletedTask;

        foreach (var error in Configuration.Errors)
        {
            Logger.Write(LogEventLevel.Fatal, error);
        }

        Environment.ExitCode = 1;

        return Task.CompletedTask;
    }
}
