using DomainModels.Types.Messages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourceingSqlDb.DbModel
{
    public abstract class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual Guid Id { get; set; }

        public virtual Guid ExecutionId { get; set; }

        public virtual Guid CorrelateId { get; set; }

        public virtual long Timestamp { get; set; }

        //Footer
        public virtual string Sender { get; set; }

        public virtual string Assembly { get; set; }

        public virtual string Environemnt { get; set; }

        public virtual string FingerPrint { get; set; }

        public virtual ResponseHint Hint { get; set; }
    }
}
