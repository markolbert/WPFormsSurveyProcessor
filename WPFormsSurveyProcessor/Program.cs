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

using J4JSoftware.DeusEx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace J4JSoftware.WpFormsSurvey;

internal class Program
{
    static void Main( string[] args )
    {
        var deusEx = new DeusEx();

        if( !deusEx.Initialize() )
        {
            J4JDeusEx.Logger?.Fatal("Could not initialize application");
            Environment.ExitCode = 1;
            return;
        }

        var service = GetService();

        var cancellationTokenSrc = new CancellationTokenSource();
        service.StartAsync( cancellationTokenSrc.Token );
    }

    private static IHostedService GetService()
    {
        var config = J4JDeusEx.ServiceProvider.GetRequiredService<Configuration>();

        if (config.Errors != null)
            return J4JDeusEx.ServiceProvider.GetRequiredService<MisconfigurationService>();

        if (config.DisplayFormInfo)
            return J4JDeusEx.ServiceProvider.GetRequiredService<FormInfoService>();

        if (config.ShowHelp)
            return J4JDeusEx.ServiceProvider.GetRequiredService<HelpService>();

        if (config.ShowDocumentation)
            return J4JDeusEx.ServiceProvider.GetRequiredService<DocumentationService>();

        return J4JDeusEx.ServiceProvider.GetRequiredService<ParseService>();
    }
}