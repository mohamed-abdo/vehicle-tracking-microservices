using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    [Flags]
    public enum Status
    {
        Idle = 1,
        Active = 2,
        Stopped = 4,
        Warnning = 8,
    }
}
