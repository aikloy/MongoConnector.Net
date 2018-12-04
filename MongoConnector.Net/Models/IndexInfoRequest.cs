using Newtonsoft.Json;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class IndexInfoRequest
    {
        [JsonProperty("column_names")]
        public List<String> ColumnNames { get; set; } = new List<string>();
    }
}
