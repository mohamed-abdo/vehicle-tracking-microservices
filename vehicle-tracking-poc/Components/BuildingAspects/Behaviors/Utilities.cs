using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

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
                ConstructorHandling= ConstructorHandling.Default,
                FloatFormatHandling= FloatFormatHandling.DefaultValue,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter(camelCaseText: true) }
            };

        public static JsonLoadSettings DefaultJsonLoadSettings =>
            new JsonLoadSettings()
            {
                LineInfoHandling= LineInfoHandling.Ignore,
                CommentHandling= CommentHandling.Ignore
            };
    }
}
