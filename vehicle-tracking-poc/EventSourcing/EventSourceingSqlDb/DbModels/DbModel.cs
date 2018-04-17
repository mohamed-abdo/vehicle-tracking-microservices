using BuildingAspects.Behaviors;
using DomainModels.System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSourceingSQLDB.DbModels
{
    public class DbModel
    {
        public DbModel() { }

        public DbModel(DbModel model)
        {
            ExecutionId = model.ExecutionId;
            CorrelationId = model.CorrelationId;
            Timestamp = model.Timestamp;

            Data = model.Data;

            Sender = model.Sender;
            Route = model.Route;
            Environment = model.Environment;
            Assembly = model.Assembly;
            FingerPrint = model.FingerPrint;
            Hint = model.Hint;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual long Id { get; set; }

        public virtual Guid ExecutionId { get; set; }

        public virtual Guid CorrelationId { get; set; }

        public virtual long Timestamp { get; set; }

        //Footer
        public virtual string Sender { get; set; }

        public virtual string Assembly { get; set; }

        public virtual string Environment { get; set; }

        public virtual string FingerPrint { get; set; }

        public virtual string Hint { get; set; }

        //TODO: add try catch to make it safe for getter and setter, by use lambda Func
        [NotMapped]
        public IDictionary<string, string> Route
        {
            get => JsonConvert.DeserializeObject<IDictionary<string, string>>(Raw_Route ?? string.Empty, Defaults.JsonSerializerSettings);
            set => Raw_Route = JsonConvert.SerializeObject(value, Defaults.JsonSerializerSettings);
        }
        [NotMapped]
        public virtual JObject Data
        {
            get => JObject.Parse(Raw_Data ?? Identifiers.DefaultJsonObject, Defaults.JsonLoadSettings);
            set => Raw_Data = JsonConvert.SerializeObject(value, Defaults.JsonSerializerSettings);
        }

        public string Raw_Data { get; set; }
        public string Raw_Route { get; set; }
    }
}
