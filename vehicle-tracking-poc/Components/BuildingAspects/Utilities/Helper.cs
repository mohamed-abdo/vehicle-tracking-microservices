using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Utilities
{
    public static class Helper
    {
        public static Func<string, (string hostName, int? port)> ExtractHostStructure = (hostName) =>
          {
              (string hostName, int? port) hostStructure = (hostName, null);
              if (hostName.Contains(":"))
              {
                  if (int.TryParse(hostName.Split(':')[1], out int portNumber))
                      hostStructure.port = portNumber;
                  else
                      throw new ArgumentException("middleware port number is not valid");
                  hostStructure.hostName = hostName.Split(':')[0];
              }
              return hostStructure;
          };
    }
}
