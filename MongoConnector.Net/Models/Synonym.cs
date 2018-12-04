using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class Synonym
    {
        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("keyword")]
        public String Keyowrd { get; set; }

        [JsonProperty("words")]
        public List<String> Words { get; set; } = new List<string>();
    }
}
