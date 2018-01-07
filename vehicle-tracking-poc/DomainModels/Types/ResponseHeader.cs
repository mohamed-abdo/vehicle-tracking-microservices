using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public class ResponseHeader : MessageHeader
    {
        public bool IsSucceed { get; set; }
        public int ResultCode { get; set; }
        public ResponseHint Hint { get; set; }
        public string Message { get; set; }
    }
}
