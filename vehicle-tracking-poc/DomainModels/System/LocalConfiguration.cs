using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DomainModels.System
{
    public sealed class LocalConfiguration
    {
        private static volatile object _sync = new object();
        private LocalConfiguration() { }
        //initialize read
        public static LocalConfiguration Create(IDictionary<string, string> configuration)
        {
            lock (_sync)
            {
                var _instance = new LocalConfiguration();
                //TODO: runtime metadata filling configuration for this object.
                foreach (var item in configuration ?? throw new ArgumentNullException($"configuration is required."))
                {
                    var property = _instance.GetType().GetProperty(item.Key) ??
                            throw new KeyNotFoundException($"{item.Key} is not recognized as configuration field.");
                    property.SetValue(obj: _instance, value: item.Value);
                }
                return _instance;
            }
        }

        #region fields
        public string CacheServer { get; private set; }
        public string MessagesMiddleware { get; private set; }
        public string CacheDBVehicles { get; private set; }
        public string MiddlewareExchange { get; private set; }
        public string MessageSubscriberRoute { get; private set; }
        public string MessagePublisherRoute { get; private set; }
        public string MessagesMiddlewareUsername { get; private set; }
        public string MessagesMiddlewarePassword { get; private set; }
        #endregion
    }
}
