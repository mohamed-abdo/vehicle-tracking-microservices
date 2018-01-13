using DomainModels.Vehicle;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.Functors
{
    public static class Validators
    {
        //TODO:we only validate the model, domain message (header / footer) validation should carry on a higher level.
        public static Action<PingModel> EnshurePingModel = (pingModel) =>
        {
            //TODO:business validation
        };
    }
}
