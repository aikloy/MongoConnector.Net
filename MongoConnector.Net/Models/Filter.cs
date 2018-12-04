using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class Filter
    {
        [BsonElement("includes")]
        public FilterInfo[] Includes { get; set; }

        [BsonElement("excludes")]
        public FilterInfo[] Excludes { get; set; }
    }
}
