using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoConnector.Net.Clients
{
    public static class MongoDriverClient
    {
        private const string connectionString = "mongodb://localhost:20170/admin";

        private static MongoClient _mongoClient = new MongoClient(connectionString);

        public static IAsyncCursor<BsonDocument> GetDatabaseList()
        {
            var cursor = _mongoClient.ListDatabases();

            return cursor;
        }

        public static IMongoDatabase GetDatabase(String name)
        {
            IMongoDatabase database = _mongoClient.GetDatabase(name);

            return database;
        }

        public static IAsyncCursor<BsonDocument> GetCollections(IMongoDatabase database)
        {
            var cursor = database.ListCollections();

            return cursor;
        }

        public static List<T> FindAll<T>(IMongoDatabase database, String name)
        {
            var collection = database.GetCollection<T>(name);

            BsonDocument filter = new BsonDocument();

            var datas = collection.Find(filter).ToList();

            return datas;
        }

        public static List<T> Find<T>(IMongoDatabase database, FilterDefinition<T> filter, String name)
        {
            var collection = database.GetCollection<T>(name);

            var datas = collection.Find(filter).ToList();

            return datas;
        }
    }
}
