using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DomainModels.System
{
    public sealed class LocalConfiguration
    {
        private volatile static LocalConfiguration _instance;
        private static object syncRoot = new object();
        private LocalConfiguration() { }

        //initialize read
        public static LocalConfiguration CreateSingletone(IDictionary<string, string> configuration)
        {
            //TODO: runtime metadata filling configuration for singletone object.

            if (_instance == null)
            {
                lock (syncRoot)
                {
                    _instance = new LocalConfiguration();

                    foreach (var item in configuration ?? throw new ArgumentNullException($"configuration is required."))
                    {
                        var property = _instance.GetType().GetProperty(item.Key) ??
                                throw new KeyNotFoundException($"{item.Key} is not recognized as configuration field.");
                        property.SetValue(obj: _instance, value: item.Value);
                    }
                }
            }
            return _instance;
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
