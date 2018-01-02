using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public sealed class Message<T>
    {
        public MessageHeader Header { get; set; }

        public T Body { get; set; }

        public MessageFooter Footer { get; set; }
    }
}
