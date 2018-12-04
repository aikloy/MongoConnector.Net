using MongoConnector.Net.Clients;
using MongoConnector.Net.Model;
using MongoConnector.Net.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoConnector.Net
{
    public class ConnectorIntermediary
    {
        private NESTClient _nestClient = new NESTClient();

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings();

        public ConnectorIntermediary()
        {
            _jsonSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }

        public void Initlization()
        {
            var cursor = MongoDriverClient.GetDatabaseList();

            foreach(var database in cursor.ToEnumerable())
            {
                if(database["name"] == null)
                {
                    Console.WriteLine("database name is null");
                    continue;
                }

                String databaseName = database["name"].ToString();

                var mongoDatabase = MongoDriverClient.GetDatabase(databaseName);
                var collectionCursor = MongoDriverClient.GetCollections(mongoDatabase);

                foreach(var collection in collectionCursor.ToEnumerable())
                {
                    String collectionName = collection["name"].ToString();

                    Console.WriteLine($"Initlization");
                    Console.WriteLine($"databaseName : {databaseName}");
                    Console.WriteLine($"collectionName : {collectionName}");
                    Console.WriteLine($"===========================================\n\n");

                    String typeName = collectionName.ToLower();
                    String indexName = databaseName.ToLower() + "." + typeName;

                    var documents = MongoDriverClient.FindAll<BsonDocument>(mongoDatabase, collectionName);

                    Console.WriteLine($"document count : {documents.Count}");

                    InitES(indexName, typeName, documents);
                }
            }
        }

        private SynonymRequest GetSynonymRequest(BsonDocument document)
        {
            SynonymRequest synonymRequest = new SynonymRequest();

            String id = String.Empty;
            if(document.Contains("_id"))
            {
                synonymRequest.Id = document["_id"].ToString();
            }
            else if(document.Contains("id"))
            {
                synonymRequest.Id = document["id"].ToString();
            }

            synonymRequest.Keyword = document["synonym_keyword"].AsString;

            foreach(var wordInfo in document["synonym_words"].AsBsonArray)
            {
                synonymRequest.Words.Add(wordInfo["word"].AsString);
            }

            return synonymRequest;
        }

        private List<SynonymRequest> GetSynonymRequest(List<BsonDocument> documents)
        {
            List<SynonymRequest> synonymRequests = new List<SynonymRequest>();

            foreach (BsonDocument document in documents)
            {
                SynonymRequest synonymRequest = GetSynonymRequest(document);

                synonymRequests.Add(synonymRequest);
            }

            return synonymRequests;
        }

        private Boolean ExistIndex(string indexName, string typeName)
        {
            return _nestClient.ExistIndex(indexName, typeName);
        }

        private void InitES(string indexName, string typeName, List<BsonDocument> documents)
        {
            if (_nestClient.ExistIndexAndCreateIndex(indexName, typeName) == true)
            {
                List<Dictionary<String, Object>> docList = new List<Dictionary<String, Object>>();

                Parallel.ForEach(documents, document =>
                {
                    String documentId = document["_id"].ToString();
                    Dictionary<String, Object> doc = document.ToDictionary();
                    doc.Remove("_id");
                    doc.Add("id", documentId);

                    if (_nestClient.ExistDocument(indexName, typeName, documentId) == false)
                    {
                        docList.Add(doc);
                    }
                }
                );

                if (docList.Count > 0)
                {
                    _nestClient.CreateBulkDocument(indexName, typeName, docList);
                }

                Console.WriteLine($"=======================\n");
                Console.WriteLine($"IndexName : {indexName}\t TypeName : {typeName}\t document Count : {documents.Count}");
                Console.WriteLine($"=======================\n");
            }
        }

        private String ReorganizeId(IDictionary<string, object> documentDict)
        {
            String id = String.Empty;
            if (documentDict.ContainsKey("_id"))
            {
                id = documentDict["_id"].ToString();
                documentDict.Remove("_id");
                documentDict.Add("id", id);
            }
            else if (documentDict.ContainsKey("id"))
            {
                id = documentDict["id"].ToString();
            }

            if (id == String.Empty)
            {
                Console.WriteLine($"id is not found");
                return null;
            }

            return id;
        }

        internal BsonValue Run(List<OpLog> oplogs)
        {
            BsonValue lastInsertDate = null;

            foreach(var document in oplogs)
            {
                if (document.Ns != String.Empty && document.IsValid() == true)
                {
                    String[] idices = document.Ns.ToLower().Split(".");

                    if (idices.Length > 1)
                    {
                        String indexName = idices[0];
                        String typeName = idices[1];


                        indexName = indexName + "." + typeName;

                        IDictionary<string, object> documentDict = ((System.Dynamic.ExpandoObject)document.O);

                        String id = ReorganizeId(documentDict);
                        if (id == String.Empty)
                            continue;

                        Console.WriteLine($"Run Id : {id}");

                        RunES(indexName, typeName, id, document.Op, document.O);
                    }
                }
                else
                {
                    Console.WriteLine($"document isValid Failed : {document.ToJson()}\n");
                }

                lastInsertDate = document.Ts;
            }

            return lastInsertDate;
        }

        internal BsonValue Run(IAsyncCursor<OpLog> cursor)
        {
            BsonValue lastInsertDate = null;

            //foreach(var document in cursor.Current)
            cursor.ForEachAsync<OpLog>(document =>
            {
                if (document.Ns != String.Empty && document.IsValid() == true)
                {
                    String[] idices = document.Ns.ToLower().Split(".");
                    if (idices.Length > 1)
                    {
                        String indexName = idices[0];
                        String typeName = idices[1];

                        indexName = indexName + "." + typeName;

                        IDictionary<string, object> documentDict = ((System.Dynamic.ExpandoObject)document.O);

                        String id = documentDict["_id"].ToString();
                        documentDict.Remove("_id");
                        documentDict.Add("id", id);
                        RunES(indexName, typeName, id, document.Op, document.O);
                    }

                }

                lastInsertDate = document.Ts;
            });

            return lastInsertDate;
        }

        private void RunES(string indexName, string typeName, string id, String op, object obj)
        {
            if (_nestClient.ExistIndexAndCreateIndex(indexName, typeName) == true)
            {
                switch (op)
                {
                    case "i":
                    case "u":
                        {
                            if (_nestClient.ExistDocument(indexName, typeName, id) == true)
                            {
                                _nestClient.UpdateDocument(indexName, typeName, id, obj);
                            }
                            else
                            {
                                _nestClient.CreateDocument(indexName, typeName, id, obj);
                            }
                        }
                        break;
                    case "d":
                        {
                            if (_nestClient.ExistDocument(indexName, typeName, id) == true)
                            {
                                _nestClient.DeleteDocument(indexName, typeName, id);
                            }
                        }
                        break;
                }

            }
        }
    }
}
