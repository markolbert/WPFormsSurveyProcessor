﻿// Copyright (c) 2022 Mark A. Olbert 
// all rights reserved
// This file is part of WpFormsSurvey.
//
// WpFormsSurvey is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// WpFormsSurvey is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with WpFormsSurvey. If not, see <https://www.gnu.org/licenses/>.

using System.Text.Json;
using System.Text.Json.Serialization;
using J4JSoftware.Logging;

namespace J4JSoftware.WpFormsSurvey;

public class WpFormsParser : WpParserBase<FieldBase>
{
    public WpFormsParser(
        IJ4JLogger logger
    )
    : base(logger)
    {
    }

    public FormsDownload? ParseFile(string filePath)
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

        var retVal = new FormsDownload();

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
                    retVal.Header = JsonSerializer.Deserialize<Header>(objText, options);
                    break;

                case "database":
                    retVal.Database = JsonSerializer.Deserialize<Database>(objText, options);
                    break;

                case "table":
                    retVal.Table = JsonSerializer.Deserialize<Forms>(objText, options);
                    break;

                default:
                    Logger.Warning<string>("Unexpected download header type '{0}' encountered", type);
                    break;
            }
        }

        if (!retVal.IsValid)
            Logger.Error("Survey download failed to parse completely");
        else ParseForms(retVal);

        return retVal;
    }

    private void ParseForms(FormsDownload download)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        foreach (var formDef in download.Table!.Data!)
        {
            // only read wpforms types with defined content
            if( ( !formDef.PostType?.Equals( "wpforms", StringComparison.OrdinalIgnoreCase ) ?? true )
            || formDef.PostContent == null )
                continue;

            var fieldsContent = JsonSerializer.Deserialize<FieldsContent>(formDef.PostContent, options);

            // for some strange reason, some WpForms forms have the Fields object as a JsonArray, and some 
            // have it as a JsonObject. We need to accomodate both
            var fieldDefEnumerator = fieldsContent!.Fields.ValueKind switch
            {
                JsonValueKind.Array => EnumerateFieldsArray( fieldsContent.Fields, options ),
                JsonValueKind.Object => EnumerateFieldsObject( fieldsContent.Fields, options ),
                _ => UnsupportedEnumerator( fieldsContent.Fields.ValueKind )
            };

            foreach (var fieldDef in fieldDefEnumerator)
            {
                if (!EntityTypes.ContainsKey(fieldDef.Type))
                {
                    Logger.Warning<string>("No field type is registered for key '{0}'", fieldDef.Type);
                    continue;
                }

                var newField = (FieldBase?)EntityTypes[fieldDef.Type]
                                       .DeserializerInfo.Invoke(null, new object[] { fieldDef.FieldText, options });

                if (newField == null)
                    Logger.Error<string>("Failed to parse survey field, type '{0}'", fieldDef.Type);
                else
                {
                    if (!newField.Initialize(formDef))
                        Logger.Error<string>("{0} field failed to initialize", fieldDef.Type);
                    else formDef.Fields.Add(newField);
                }
            }
        }
    }
}
