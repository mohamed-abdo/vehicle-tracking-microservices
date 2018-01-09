using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModels.Types.Exceptions
{
    /// <summary>
    /// Exception code to be used when throwing exceptions
    /// </summary>
    [Flags]
    public enum ExceptionCodes
    {
        #region system exception => start from 1000 up to 1999

        [ExceptionCode(1000)]
        InternalError,

        #endregion

        #region messages & services exception => start from 2000 up to 2999

        [ExceptionCode(2000, friendlyMessage: "Message malformed, or is not complaint with system messages types.")]
        MessageMalformed,

        #endregion

        #region business rules exception => start from 3000 up to 3999


        #endregion

        #region data store exception => start from 4000 up to 4999


        #endregion

        #region UI exception => start from 5000 up to 5999


        #endregion
    }

    public static class ExceptionCodesHelper
    {
        public static (int code, string message, ResponseHint hint) Translate(ExceptionCodes code)
        {
            return code.GetType()
            .GetField(Enum.GetName(typeof(ExceptionCodes), code))
            .GetCustomAttributes(false).Where((attr) =>
            {
                return (attr is ExceptionCodeAttribute);
            }).Select(customAttr =>
            {
                var attr = (customAttr as ExceptionCodeAttribute);
                return (attr.Code, attr.FriendlyMessage, attr.Hint);
            }).FirstOrDefault();
        }
    }
}
