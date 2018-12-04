using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Model
{
    public class OpLog
    {
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("ts")]
        public BsonTimestamp Ts { get; set; }

        [BsonElement("t")]
        public Int64 T { get; set; }

        [BsonElement("h")]
        public Int64 H { get; set; }

        [BsonElement("v")]
        public Int32 V { get; set; }

        [BsonElement("op")]
        public String Op { get; set; }

        [BsonElement("ns")]
        public String Ns { get; set; }

        [BsonElement("ui")]
        public Object Ui { get; set; }

        [BsonElement("wall")]
        public Object Wall { get; set; }

        [BsonElement("o2")]
        public Object O2 { get; set; }

        [BsonElement("o")]
        public Object O { get; set; }

        public Boolean IsValid()
        {
            if(Ns.Contains("$cmd") || Ns.Contains("_unused_"))
            {
                return false;
            }

            return true;
        }
    }
}
