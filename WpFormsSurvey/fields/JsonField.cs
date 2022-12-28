using System.Text.Json;

namespace J4JSoftware.WpFormsSurvey;

public class JsonField
{
    protected JsonField()
    {
    }

    protected IEnumerable<TTarget?> EnumerateObject<TTarget>(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            yield return JsonSerializer.Deserialize<TTarget>(fieldText, options);
        }
    }

    protected IEnumerable<TTarget?> EnumerateArray<TTarget>(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateArray())
        {
            var fieldText = fieldObj.GetRawText();

            yield return JsonSerializer.Deserialize<TTarget>(fieldText, options);
        }
    }

    protected IEnumerable<TTarget> UnsupportedEnumerator<TTarget>(JsonValueKind valueKind)
    {
        yield break;
    }
}
