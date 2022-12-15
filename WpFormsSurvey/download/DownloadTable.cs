using WPFormsSurvey;

namespace WpFormsSurvey;

public record DownloadTable( string Name, string Database, List<FormDefinition>? Data );
