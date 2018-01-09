using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public interface IMessageFooter
    {
        string Sender { get; }
        string Assembly { get; }
        string Environemnt { get; }
        IDictionary<string, string> Route { get; }
        string FingerPrint { get; }
        MessageHint Hint { get; }
    }
}
