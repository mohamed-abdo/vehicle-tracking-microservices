using System.Collections.Generic;

namespace DomainModels.Types
{
    public class MessageFooter : IMessageFooter
    {
        public string Sender { get; set; }

        public string Assembly { get; set; }

        public string Environemnt { get; set; }

        public IDictionary<string, string> Route { get; set; }

        public string FingerPrint { get; set; }

        public MessageHint Hint { get; set; }
    }
}
