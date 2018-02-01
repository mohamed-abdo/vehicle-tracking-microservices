using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ping.Models
{
    [Flags]
    public enum VehicleStatus
    {
        active = 1,
        inactive = 2,
        warning = 4,
        critical = 8,
    }
}
