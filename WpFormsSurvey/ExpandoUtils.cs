using System.Dynamic;
using J4JSoftware.Logging;

namespace WpFormsSurvey;

public static class ExpandoUtils
{
    public static bool HasElement(ExpandoObject toCheck, string name) =>
        ((IDictionary<string, object?>)toCheck).ContainsKey(name);
}
