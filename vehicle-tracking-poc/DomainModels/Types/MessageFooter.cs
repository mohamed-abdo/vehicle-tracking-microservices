using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public class MessageFooter : IMessageFooter
    {
        public Guid SenderId { get; set; }

        public string Route { get; set; }

        public string FingerPrint { get; set; }
    }
}
