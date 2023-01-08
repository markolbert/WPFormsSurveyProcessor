# J4JSoftware.WpFormsSurveyProcessor

|Version|Description|
|:-----:|-----------|
|1.1.0|fix bugs, add new command line option; [see details below](#110)|
|1.0.0|initial release|

## 1.1.0

Fixed a bug in how the application configuration file was being located at runtime.

Added a new command line option, /l, to enable specifying the logging level used to display messages. Acceptable values are the enumerated values for [Serilog's log event level](https://github.com/serilog/serilog/blob/main/src/Serilog/Events/LogEventLevel.cs). The default is Warning.
