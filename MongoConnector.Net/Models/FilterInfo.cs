using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class FilterInfo
    {
        [BsonElement("index_name")]
        public String IndexName { get; set; }

        [BsonElement("columns")]
        public String[] Columns { get; set; }
    }
}
