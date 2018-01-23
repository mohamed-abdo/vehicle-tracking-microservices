using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    [Serializable]
    [Flags]
    public enum StatusModel
    {
        Active = 1,
        InActive = 2,
        Warnning = 4,
        Critical = 8,
        Unknown = 16
    }
}
