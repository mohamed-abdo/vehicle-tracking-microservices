using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Services
{

    public sealed class OperationalUnit : IOperationalUnit
    {
        private readonly string _InstanceId;
        private readonly string _Envionment;
        private readonly string _Assembly;

        public OperationalUnit(string environment, string assembly)
        {
            _InstanceId = Guid.NewGuid().ToString();
            _Envionment = environment;
            _Assembly = assembly;
        }
        public string InstanceId => _InstanceId;

        public string Environment => _Envionment;

        public string Assembly => _Assembly;

    }
}
