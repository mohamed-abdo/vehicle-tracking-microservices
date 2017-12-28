using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public interface IMessageFooter
    {
        Guid SenderId { get; }
        string Route { get; }
        string FingerPrint { get; }
    }
}
