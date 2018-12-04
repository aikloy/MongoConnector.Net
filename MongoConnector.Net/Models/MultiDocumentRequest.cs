using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Models
{
    public class MultiDocumentRequest
    {
        [JsonProperty("documents")]
        public List<Dictionary<string, object>> Documents { get; set; } = new List<Dictionary<string, object>>();

        //[JsonProperty("document_ids")]
        //public List<String> DocumentIds { get; set; } = new List<String>();
    }
}
