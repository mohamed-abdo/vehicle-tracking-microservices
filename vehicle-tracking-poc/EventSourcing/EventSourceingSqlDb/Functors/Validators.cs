using DomainModels.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.Functors
{
    public static class Validators<T>
    {
        //TODO:we only validate the model, domain message (header / footer) validation should carry on a higher level.
        public static Action<T> EnshureModel = (model) =>
        {
            //TODO:business validation
        };
    }
}
