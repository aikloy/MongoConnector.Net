using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class SynonymRequest
    {
        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("keyword")]
        public String Keyword { get; set; }

        [JsonProperty("words")]
        public List<String> Words { get; set; } = new List<string>();
    }
}
