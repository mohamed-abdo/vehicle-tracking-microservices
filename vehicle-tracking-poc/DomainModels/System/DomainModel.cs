using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public class DomainModel<T>
    {
        public readonly int Id;
        public string Name { get; set; }
        public T Model { get; set; }
    }
}
