using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Clients
{
    public class NESTClient
    {
        private ElasticClient _elasticClient = null;
        private const String _url = "http://localhost:9200";

        public NESTClient()
        {
            var node = new Uri(_url);
            var settings = new ConnectionSettings(node);
            _elasticClient = new ElasticClient(settings);
        }

        public Boolean ExistType(String index, String type)
        {
            var existsResponse = _elasticClient.TypeExists(index, type);
            if (existsResponse.Exists == false)
            {
                return false;
            }
            
            return true;
        }

        public Boolean ExistIndex(String index, String type)
        {
            // https://www.elastic.co/guide/en/elasticsearch/reference/current/removal-of-types.html
            return _elasticClient.IndexExists(index).Exists;
        }

        public Boolean CreateIndex(String index, String type)
        {
            CreateIndexRequest createRequest = new CreateIndexRequest(index);
            createRequest.Mappings = new Mappings();

            ITypeMapping typeMapping = new TypeMapping();

            createRequest.Mappings.Add(type, typeMapping);

            var createIndexResponse = _elasticClient.CreateIndex(createRequest);

            return createIndexResponse.IsValid;
        }

        public Boolean ExistIndexAndCreateIndex(String index, String type)
        {
            // https://www.elastic.co/guide/en/elasticsearch/reference/current/removal-of-types.html
            if (_elasticClient.IndexExists(index).Exists == false)
            {
                CreateIndexRequest createRequest = new CreateIndexRequest(index);
                createRequest.Mappings = new Mappings();

                ITypeMapping typeMapping = new TypeMapping();

                createRequest.Mappings.Add(type, typeMapping);
                
                var createIndexResponse = _elasticClient.CreateIndex(createRequest);

                return createIndexResponse.IsValid;
            }

            return true;
        }

        public Boolean CreateBulkDocument(String index, String type, List<Dictionary<String, Object>> obj)
        {
            var descriptor = new BulkDescriptor();

            descriptor.CreateMany<Dictionary<String, Object>>(obj, (bd, q) => bd.Id(q["id"].ToString()).Index(index).Type(type));

            var bulkResponse = _elasticClient.Bulk(descriptor);

            var response = _elasticClient.BulkAll(obj, idx => idx
                .Index(index)
                .Type(type)
                .Refresh(Refresh.True)
            );

            if (bulkResponse.ServerError != null && bulkResponse.ServerError.ToString().Length > 0)
            {
                Console.WriteLine("Error : {0}", bulkResponse.ServerError.ToString());
                Console.WriteLine("Error Content : {0}", Newtonsoft.Json.JsonConvert.SerializeObject(obj));
            }

            return bulkResponse.IsValid;
        }
        public Boolean CreateDocument(String index, String type, string id, object obj)
        {
            var response = _elasticClient.Index(obj, idx => idx
                .Index(index)
                .Type(type)
                .Id(id)
                .Refresh(Refresh.True)
            );

            if (response.ServerError != null && response.ServerError.ToString().Length > 0)
            {
                Console.WriteLine("Error : {0}", response.ServerError.ToString());
                Console.WriteLine("Error Content : {0}", Newtonsoft.Json.JsonConvert.SerializeObject(obj));
            }

            return response.IsValid;
        }

        public Boolean UpdateDocument(String index, String type, string id, object obj)
        {
            var updateResponse = _elasticClient.Update<Object>(id, idx => idx.Index(index).Type(type).Upsert(obj) );

            return updateResponse.IsValid;
        }

        public Boolean DeleteDocument(String index, String type, string id)
        {
            var deleteResponse = _elasticClient.Delete<Object>(id, idx => idx.Index(index).Type(type));

            return deleteResponse.IsValid;
        }

        public Boolean ExistDocument(String index, String type, string id)
        {
            var existDocument = _elasticClient.DocumentExists<Object>(id, idx=> idx.Index(index).Type(type));

            return existDocument.Exists;
        }
    }
}
