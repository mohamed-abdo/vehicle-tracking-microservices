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
        public ExceptionCodeAttribute(int code, string friendlyMessage="Internal error.")
        {
            _Code = code;
            _friendlyMessage = friendlyMessage;
        }
        public int Code => _Code;
        public string FriendlyMessage => _friendlyMessage;
    }
}
