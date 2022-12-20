using System.Collections;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

public record SubmissionInfo( List<Form> Forms, List<IndividualSubmission> UserSubmissions );
