using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Behaviors
{
    public static class Defaults
    {
        public static JsonSerializerSettings JsonSerializerSettings =>
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

        public static JsonLoadSettings JsonLoadSettings =>
            new JsonLoadSettings()
            {
                LineInfoHandling = LineInfoHandling.Ignore,
                CommentHandling = CommentHandling.Ignore
            };

        public static TimeSpan CacheTimeout => TimeSpan.FromMinutes(1);
    }
}
