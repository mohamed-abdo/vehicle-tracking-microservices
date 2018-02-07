using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DomainModels.System
{
    public sealed class LocalConfiguration : InfrastructureConfiguration
    {
        private static volatile object _sync = new object();
        //initialize read
        public override InfrastructureConfiguration Create(IDictionary<string, string> configuration)
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
        
    }
}
