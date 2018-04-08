using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Services
{
    public interface IOperationalUnit
    {
       //TODO:though eligible to use as a correlation id for operation within instance
       string InstanceId { get; }
       string Environment { get; }
       string Assembly { get; }
    }
}
