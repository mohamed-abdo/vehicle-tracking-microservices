using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.DataStructure
{
    public struct RabbitMQConfiguration
    {
        public string hostName;
        public string userName;
        public string password;
        public string exchange;
        public string[] routes;
    }
}
