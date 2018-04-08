using System;
using System.Collections.Generic;

namespace DomainModels.Types.Messages
{
    [Serializable]
    public class MessageFooter : IMessageFooter
    {
        public string Sender { get; set; }

        public string Assembly { get; set; }

        public string Environment { get; set; }

        public string Route { get; set; }

        public string FingerPrint { get; set; }

        public string Hint { get; set; }
    }
}
