using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.Configuration.J4JCommandLine;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class FormInfoService : ServiceBase
{
    public FormInfoService(
        Configuration config,
        IJ4JLogger logger
    )
        : base( config, logger )
    {
    }
}
