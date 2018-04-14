using DomainModels.Types.Messages;
using System;
using System.Runtime.Serialization;

namespace DomainModels.Types
{

    public interface IDomainModel<Model>
    {
        MessageHeader Header { get; set; }
        Model Body { get; set; }
        MessageFooter Footer { get; set; }
    }
    [Serializable]
    public class DomainModel<Model> : IDomainModel<Model>
    {
        public MessageHeader Header { get; set; }
        public Model Body { get; set; }
        public MessageFooter Footer { get; set; }
    }
}
