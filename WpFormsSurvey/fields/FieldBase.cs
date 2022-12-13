using System.Text.Json;
using J4JSoftware.Logging;

namespace WPFormsSurvey;

public class FieldBase
{
    protected FieldBase()
    {
    }

    public int Id { get; set; }
    public List<FieldConditional> Conditionals { get; set; } = new();
}
