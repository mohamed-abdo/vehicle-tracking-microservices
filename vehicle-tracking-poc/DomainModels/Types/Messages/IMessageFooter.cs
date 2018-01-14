using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public interface IMessageFooter
    {
        string Sender { get; }
        string Assembly { get; }
        string Environment { get; }
        IDictionary<string, string> Route { get; }
        string FingerPrint { get; }
        ResponseHint Hint { get; }
    }
}
