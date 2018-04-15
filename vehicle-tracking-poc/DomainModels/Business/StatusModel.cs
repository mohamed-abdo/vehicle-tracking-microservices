using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Business
{
    [Serializable]
    [Flags]
    public enum StatusModel
    {
        active = 1,
        inActive = 2,
        warnning = 4,
        critical = 8,
        unknown = 16
    }
}
