using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourceingSqlDb.DbModel
{
    public abstract class BaseDbModel
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

        //TODO: add try catch to make it safe for getter and setter, by use lambda Func
        [NotMapped]
        public IDictionary<string, string> Route
        {
            get => JsonConvert.DeserializeObject<IDictionary<string, string>>(Raw_Route, Utilities.DefaultJsonSerializerSettings);
            set => JsonConvert.SerializeObject(value, Utilities.DefaultJsonSerializerSettings);
        }

        public string Raw_Route { get; set; }
    }
}
