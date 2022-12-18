using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal partial class DeusEx
{
    private void ConfigureDependencyInjection( HostBuilderContext hbc, ContainerBuilder builder )
    {
        builder.RegisterType<WpFormsParser>()
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<WpResponsesParser>()
               .AsSelf()
               .SingleInstance();

        builder.Register( c => hbc.Configuration.Get<Configuration>() ?? new Configuration() )
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<MisconfigurationService>()
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<HelpService>()
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<ParseService>()
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<FormInfoService>()
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<ExportFormInfo>()
               .AsSelf()
               .SingleInstance();
    }
}
