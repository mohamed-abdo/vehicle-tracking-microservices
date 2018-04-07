using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    /// <summary>
    /// Describe core model properties in string format.
    /// </summary>
    public interface IDescribe
    {
        string Description { get; }
    }
}
