using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    [Flags]
    public enum ResponseHint
    {
        OK = 1,
        Retry = 2,
        CorrectInput = 4,
        InMaintenance = 8,
        SystemError = 16,
        Deprecated = 32,
        Obsoleted = 64
    }
}
