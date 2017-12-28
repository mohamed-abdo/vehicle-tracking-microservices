using DomainModels.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildingAspects.Functors
{
    /// <summary>
    /// Validate a certain logic, over model (a -> implies b)?true : throw!
    /// </summary>
    public sealed class Validators
    {
        /// <summary>
        /// Insure (validate / verify) rabbitMQ host configuration data are valid, else throw according to exceptions.
        /// </summary>
        public static Predicate<RabbitMQConfiguration> EnsureHostConfig = (config) =>
          {
              switch (config)
              {
                  case var t when string.IsNullOrEmpty(t.hostName):
                      throw new ArgumentNullException("hostName is invalid");
                  case var t when string.IsNullOrEmpty(t.exchange):
                      throw new ArgumentNullException("exchange is invalid");
                  case var t when t.routes == null || t.routes.Length == 0 || t.routes.Any(r => string.IsNullOrEmpty(r)):
                      throw new ArgumentNullException("routes is invalid");
                  default:
                      return true;
              }
          };
    }
}
