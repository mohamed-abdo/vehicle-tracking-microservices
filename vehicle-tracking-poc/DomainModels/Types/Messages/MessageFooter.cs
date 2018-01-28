using System;
using System.Collections.Generic;

namespace DomainModels.Types.Messages
{
    [Serializable]
    public class MessageFooter : DomainModel, IMessageFooter
    {
        public string Sender { get; set; }

        public string Assembly { get; set; }

        public string Environment { get; set; }

        public IDictionary<string, string> Route { get; set; }

        public string FingerPrint { get; set; }

        public ResponseHint Hint { get; set; }

        public override string Descripition => $"Sender:{Sender} Hint:{Hint}";
    }
}
