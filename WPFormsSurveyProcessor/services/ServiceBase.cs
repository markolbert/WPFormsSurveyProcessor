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

using J4JSoftware.Logging;
using Microsoft.Extensions.Hosting;

namespace J4JSoftware.WpFormsSurvey;

internal class ServiceBase : IHostedService
{
    protected ServiceBase(
        Configuration config,
        IJ4JLogger logger
    )
    {
        Configuration = config;

        Logger = logger;
        Logger.SetLoggedType( GetType() );
    }

    protected Configuration Configuration { get; }
    protected IJ4JLogger Logger { get;}

    public virtual Task StartAsync( CancellationToken cancellationToken )
    {
        Logger.Warning("{0} not implemented", GetType());

        return Task.CompletedTask;
    }

    public virtual Task StopAsync( CancellationToken cancellationToken )
    {
        return Task.CompletedTask;
    }
}
