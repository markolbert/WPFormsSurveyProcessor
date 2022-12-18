using System.Text.Json;

namespace WpFormsSurvey;

public class JsonField
{
    protected JsonField()
    {
    }

    protected IEnumerable<TTarget?> EnumerateFieldsObject<TTarget>(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            yield return JsonSerializer.Deserialize<TTarget>(fieldText, options);
        }
    }

    protected IEnumerable<TTarget?> EnumerateFieldsArray<TTarget>(JsonElement container, JsonSerializerOptions options)
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
