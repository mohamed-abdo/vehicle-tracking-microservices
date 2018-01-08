using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace BuildingAspects.Behaviors
{
    public static class Utilities
    {
        public static JsonSerializerSettings JsonSerializerSettings =>
            new Newtonsoft.Json.JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include,
                Converters = new List<JsonConverter> { new StringEnumConverter(camelCaseText: true) }
            };
    }
}
