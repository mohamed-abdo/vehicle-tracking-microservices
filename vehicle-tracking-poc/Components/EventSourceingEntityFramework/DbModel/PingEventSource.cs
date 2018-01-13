using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EventSourceingSqlDb.DbModel
{
    public class PingEventSource: BaseModel
    {
        //TODO: add try catch to make it safe for getter and setter, by use lambda Func
        [NotMapped]
        public JObject Data
        {
            get => JObject.Parse(Raw_Data, Utilities.DefaultJsonLoadSettings);
            set => JsonConvert.SerializeObject(value, Utilities.DefaultJsonSerializerSettings);
        }

        public string Raw_Data { get; set; }

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
