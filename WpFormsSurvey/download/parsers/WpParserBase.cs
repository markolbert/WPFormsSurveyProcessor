// Copyright (c) 2022 Mark A. Olbert 
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

using System.Reflection;
using System.Text.Json;
using J4JSoftware.Logging;

namespace J4JSoftware.WpFormsSurvey;

public class WpParserBase<TEntity>
    where TEntity : class
{
    protected record FieldDefinition(string Type, string FieldText);
    protected record EntityInfo(Type EntityType, MethodInfo DeserializerInfo);

    private enum EntityTypeClass
    {
        Invalid,
        NonPublic,
        Undecorated,
        Okay
    }

    protected readonly Dictionary<string, EntityInfo> EntityTypes = new(StringComparer.OrdinalIgnoreCase);
    protected readonly MethodInfo? GenericDeserializer;
    protected IJ4JLogger Logger;

    protected WpParserBase(
        IJ4JLogger logger
    )
    {
        var thisType = GetType();

        Logger = logger;
        Logger.SetLoggedType(thisType);

        var temp = typeof(JsonSerializer)
                           .GetMethods()
                           .FirstOrDefault(x =>
                            {
                                if (!x.IsGenericMethod || x.GetGenericArguments().Length != 1)
                                    return false;

                                var args = x.GetParameters();

                                if (args.Length != 2)
                                    return false;

                                if (args[0].ParameterType != typeof(string))
                                    return false;

                                return args[1].ParameterType == typeof(JsonSerializerOptions);
                            });

        if (temp == null)
            throw new ApplicationException($"Could not find {nameof(JsonSerializer.Deserialize)}");

        GenericDeserializer = temp;

        RegisterEntityTypes(thisType.Assembly);
    }

    public bool RegisterEntityType<TField>() => RegisterEntityType(typeof(TField));

    public bool RegisterEntityType(Type entityType)
    {
        if( entityType.IsAssignableTo( typeof( TEntity ) ) )
        {
            var result = RegisterFieldInternal( entityType );
            return result is EntityTypeClass.NonPublic or EntityTypeClass.Okay;
        }

        Logger.Error("{0} does not derive from {1}", entityType, typeof(TEntity));

        return false;
    }

    public bool RegisterEntityTypes(Assembly assembly)
    {
        var retVal = true;

        foreach( var type in assembly.GetTypes()
                                     .Where( x => x.IsAssignableTo( typeof( TEntity ) ) ) )
        {
            var result = RegisterFieldInternal(type);
            retVal &= result is EntityTypeClass.NonPublic or EntityTypeClass.Okay;
        }

        return retVal;
    }

    private EntityTypeClass RegisterFieldInternal(Type entityType)
    {
        var publicParameterless = entityType.GetConstructor( Type.EmptyTypes ) != null;
        var protectedParameterless =
            entityType.GetConstructor( BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes ) != null;

        if( entityType == typeof( TEntity ) || (!publicParameterless && protectedParameterless) )
            return EntityTypeClass.NonPublic;

        if (entityType.GetConstructor(Type.EmptyTypes) == null)
        {
            Logger.Warning("{0} does not have a public parameterless constructor", entityType);
            return EntityTypeClass.Invalid;
        }

        var attributes = entityType.GetCustomAttributes<WpFormsFieldTypeAttribute>(false)
                                   .ToList();
        if (!attributes.Any())
        {
            Logger.Error("{0} is not decorated with any WpFormsFieldTypeAttributes", entityType);
            return EntityTypeClass.Undecorated;
        }

        var entityInfo = new EntityInfo(entityType, GenericDeserializer!.MakeGenericMethod(entityType));

        foreach( var attribute in attributes )
        {
            if (EntityTypes.ContainsKey(attribute.EntityName))
            {
                EntityTypes[attribute.EntityName] = entityInfo;
                Logger.Information<string>("Replaced IWpFormsField for field '{0}'", attribute.EntityName);
            }
            else EntityTypes.Add(attribute.EntityName, entityInfo);
        }

        return EntityTypeClass.Okay;
    }

    protected IEnumerable<FieldDefinition> EnumerateFieldsObject(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                Logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    protected IEnumerable<FieldDefinition> EnumerateFieldsArray(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateArray())
        {
            var fieldText = fieldObj.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                Logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    protected IEnumerable<FieldDefinition> UnsupportedEnumerator(JsonValueKind valueKind)
    {
        Logger.Error("Unsupported fields ValueKind '{0}'", valueKind);
        yield break;
    }
}
