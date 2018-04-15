using DomainModels.Business;
using Ping.Models;
using System;

namespace Ping.Helper
{
    public sealed class Mappers
    {
        public static Func<VehicleStatus?, StatusModel> inferStatus = (status) =>
        {
            switch (status)
            {
                case null:
                    return StatusModel.active;//default status
                case VehicleStatus.active:
                   return StatusModel.active;
                case VehicleStatus.inactive:
                    return StatusModel.inActive;
                case VehicleStatus.warning:
                    return StatusModel.warnning;
                case VehicleStatus.critical:
                    return StatusModel.critical;
                default:
                    return StatusModel.unknown;
            }
        };
    }
}
