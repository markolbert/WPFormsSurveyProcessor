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

using System.Diagnostics;
using J4JSoftware.Logging;

namespace J4JSoftware.WpFormsSurvey;

internal class DocumentationService : ServiceBase
{
    public DocumentationService( 
        Configuration config, 
        IJ4JLogger logger 
    )
        : base( config, logger )
    {
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        var docsDir = Path.Combine( Environment.CurrentDirectory, "docs" );

        var htmlPath = Path.Combine(docsDir, "readme.html");
        if( !File.Exists( htmlPath ) )
        {
            Logger.Warning("Documentation file not available or accessible");
            return Task.CompletedTask;
        }

        var htmlProcess = new Process { StartInfo = new ProcessStartInfo( htmlPath ) { UseShellExecute = true } };
        htmlProcess.Start();

        return Task.CompletedTask;
    }
}
