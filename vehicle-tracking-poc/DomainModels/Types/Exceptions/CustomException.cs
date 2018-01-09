using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Exceptions
{
    public class CustomException : Exception
    {
        private readonly string _friendlyMessage;
        private readonly ExceptionCodes _code;
        private readonly (int code, string friendlyMessage) _message;

        public CustomException(ExceptionCodes code)
        {
            _message = ExceptionCodesHelper.Translate(code);
        }
        public CustomException((int code, string friendlyMessage) message)
        {
            _message = message;
        }

        public (int code, string friendlyMessage) CustomMessage => _message;
    }
}
