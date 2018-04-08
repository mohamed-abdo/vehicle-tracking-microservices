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
        string Route { get; }
        string FingerPrint { get; }
        string Hint { get; }
    }
}
