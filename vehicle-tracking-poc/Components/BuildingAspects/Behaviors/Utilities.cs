using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace BuildingAspects.Behaviors
{
    public static class Utilities
    {
        public static JsonSerializerSettings DefaultJsonSerializerSettings =>
            new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Include,
                ConstructorHandling = ConstructorHandling.Default,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter(camelCaseText: true) }
            };

        public static JsonLoadSettings DefaultJsonLoadSettings =>
            new JsonLoadSettings()
            {
                LineInfoHandling = LineInfoHandling.Ignore,
                CommentHandling = CommentHandling.Ignore
            };
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            var fields = source.GetType().GetFields(
                BindingFlags.GetField |
                BindingFlags.Public |
                BindingFlags.Instance).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source) ?? string.Empty
            );

            var properties = source.GetType().GetProperties(
                BindingFlags.GetField |
                BindingFlags.GetProperty |
                BindingFlags.Public |
                BindingFlags.Instance).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null) ?? string.Empty
            );

            return fields.Concat(properties).ToDictionary(key => key.Key, value => value.Value); ;
        }
        public static bool EqualsByValue(this object source, object destination)
        {
            var firstDic = source.ToFlattenDictionary();
            var secondDic = destination.ToFlattenDictionary();
            if (firstDic.Count != secondDic.Count)
                return false;
            if (firstDic.Keys.Except(secondDic.Keys).Any())
                return false;
            if (secondDic.Keys.Except(firstDic.Keys).Any())
                return false;
            return firstDic.All(pair =>
              pair.Value.Equals(secondDic[pair.Key])
            );
        }
        public static bool IsAnonymousType(this object instance)
        {

            if (instance == null)
                return false;

            return instance.GetType().Namespace == null;
        }
        public static IDictionary<string, object> ToFlattenDictionary(this object source, string parentPropertyKey = null, IDictionary<string, object> parentPropertyValue = null)
        {
            var propsDic = parentPropertyValue ?? new Dictionary<string, object>();
            foreach (var item in source.ToDictionary())
            {
                var key = string.IsNullOrEmpty(parentPropertyKey) ? item.Key : $"{parentPropertyKey}.{item.Key}";
                if (item.Value.IsAnonymousType())
                    return item.Value.ToFlattenDictionary(key, propsDic);
                else
                    propsDic.Add(key, item.Value);
            }
            return propsDic;
        }

        public static byte[] BinarySerialize(object instance)
        {
            byte[] binObjSource;
            var formatter = new BinaryFormatter();
            using (var memory = new MemoryStream())
            {
                formatter.Serialize(memory, instance);
                binObjSource = memory.ToArray();
            }
            return binObjSource;
        }
        public static object BinaryDeserialize(byte[] objAsBinary)
        {
            var formatter = new BinaryFormatter();
            using (var memory = new MemoryStream(objAsBinary))
            {
                return formatter.Deserialize(memory);
            }
        }
    }
}

