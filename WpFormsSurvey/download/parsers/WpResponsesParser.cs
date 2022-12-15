using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;

namespace WpFormsSurvey;

public class WpResponsesParser : WpParserBase<ResponseBase>
{
    public WpResponsesParser(
        IJ4JLogger logger
    )
        : base(logger)
    {
        RegisterEntityTypes(GetType().Assembly);
    }

    public ResponseDownload? ParseFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Logger.Error<string>("File '{0}' does not exist or is not accessible", filePath);
            return null;
        }

        var rawJson = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(filePath));

        if (rawJson.ValueKind != JsonValueKind.Array)
        {
            Logger.Error<string>("{0} did not parse to a JSON array", filePath);
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var retVal = new ResponseDownload();

        foreach (var element in rawJson.EnumerateArray())
        {
            var text = element.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(text, options);
            if (objType == null)
            {
                Logger.Error("Could not determine object type for header object");
                return null;
            }

            var type = objType.Type.ToLower();
            var objText = element.GetRawText();

            switch (type)
            {
                case "header":
                    retVal.Header = JsonSerializer.Deserialize<DownloadHeader>(objText, options);
                    break;

                case "database":
                    retVal.Database = JsonSerializer.Deserialize<DownloadDatabase>(objText, options);
                    break;

                case "table":
                    retVal.Table = JsonSerializer.Deserialize<DownloadResponseTable>(objText, options);
                    break;

                default:
                    Logger.Warning<string>("Unexpected download header type '{0}' encountered", type);
                    break;
            }
        }

        if (!retVal.IsValid)
            Logger.Error("Forms response download failed to parse completely");
        else ParseResponses(retVal);

        return retVal;
    }

    private void ParseResponses(ResponseDownload download)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        foreach (var responseDef in download.Table!.Data!)
        {
            var rawText = responseDef.Fields.GetRawText();
            if (string.IsNullOrEmpty(rawText  ))
                continue;

            var fieldsContent = JsonSerializer.Deserialize<FieldsContent>(rawText, options);

            // for some strange reason, some WpForms forms have the Fields object as a JsonArray, and some 
            // have it as a JsonObject. We need to accomodate both
            var respDefEnumerator = fieldsContent!.Fields.ValueKind switch
            {
                JsonValueKind.Array => EnumerateFieldsArray(fieldsContent.Fields, options),
                JsonValueKind.Object => EnumerateFieldsObject(fieldsContent.Fields, options),
                _ => UnsupportedEnumerator(fieldsContent.Fields.ValueKind)
            };

            foreach (var respDef in respDefEnumerator)
            {
                if (!EntityTypes.ContainsKey(respDef.Type))
                {
                    Logger.Warning<string>("No response type is registered for key '{0}'", respDef.Type);
                    continue;
                }

                var newResponse = (ResponseBase?)EntityTypes[respDef.Type]
                                       .DeserializerInfo.Invoke(null, new object[] { respDef.FieldText, options });

                if (newResponse == null)
                    Logger.Error<string>("Failed to parse survey response, type '{0}'", respDef.Type);
                else
                {
                    if (!newResponse.Initialize())
                        Logger.Error<string>("{0} response failed to initialize", respDef.Type);
                    else responseDef.Responses.Add(newResponse);
                }
            }
        }
    }
}
