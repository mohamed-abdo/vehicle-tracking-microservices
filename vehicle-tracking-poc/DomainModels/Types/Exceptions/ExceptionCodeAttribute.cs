using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Exceptions
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExceptionCodeAttribute : Attribute
    {
        private readonly int _Code;
        private readonly string _friendlyMessage;
        private readonly ResponseHint _responseHint;
        public ExceptionCodeAttribute(int code, string friendlyMessage = "Internal error.", ResponseHint responseHint = ResponseHint.SystemError)
        {
            _Code = code;
            _friendlyMessage = friendlyMessage;
            _responseHint = responseHint;
        }
        public int Code => _Code;
        public string FriendlyMessage => _friendlyMessage;
        public ResponseHint Hint => _responseHint;
    }
}
