# J4JSoftware.WpFormsSurveyProcessor Developer Information

This app relies on a number of libraries I wrote and maintain. You can visit their GitHub repositories by clicking on the library name:

|Library|Description|
|-------|-----------|
|[J4JLogger](https://github.com/markolbert/J4JLogging)|wrapper/extension to [Serilog](https://github.com/serilog)|
|[J4JCommandLine](https://github.com/markolbert/J4JCommandLine)|command line processor that works with the `IConfiguration` API|
|[DependencyInjection](https://github.com/markolbert/ProgrammingUtilities/tree/master/DependencyInjection)|root object resolver that works with the `IHostBuilder`/`IHost` API and [Autofac](https://autofac.org/)|
|[MiscellaneousUtilities](https://github.com/markolbert/ProgrammingUtilities/tree/master/MiscellaneousUtilities)|various utility methods|
|[ColorfulHelp](https://github.com/markolbert/ProgrammingUtilities/tree/master/MiscellaneousUtilities)|add on to `J4JCommandLine` which displays formatted command line help|
|[WpFormsSurvey](https://github.com/markolbert/WPFormsSurveyProcessor/blob/master/WpFormsSurvey/docs/readme.md)|importers for WPForms form definitions and survey responses|

The application is structured around a series of services, each mapped to a specific configuration of command line options. Click on the links for more information about the primary services:

|Service|Description|
|-------|-----------|
|`HelpService`|displays formatted help|
|`DocumentationService`|displays documentation in a browser window|
|`MisconfigurationService`|displays help and then a series of error messsages|
|[FormInfoService](form-info-service.md)|parses a downloaded WordPress posts file and displays a list of forms and their ids|
|[ParseService](parsing-service.md)|parses downloaded WordPress posts and WPForms survey responses files and creates an Excel spreadsheet|
