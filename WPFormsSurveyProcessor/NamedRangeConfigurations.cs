namespace WPFormsSurveyProcessor;

public record NamedRangeConfigurations(
    NamedRangeConfiguration? Forms,
    NamedRangeConfiguration? Fields,
    NamedRangeConfiguration? Choices,
    NamedRangeConfiguration? Submissions );