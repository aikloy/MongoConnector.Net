using MongoConnector.Net.Clients;
using MongoConnector.Net.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoConnector.Net
{
    public class MongoOpLogManager
    {
        IMongoDatabase _local = null;
        IMongoCollection<OpLog> _opLog = null;

        public MongoOpLogManager()
        {            
            _local = MongoDriverClient.GetDatabase("local");

            _opLog = _local.GetCollection<OpLog>("oplog.rs");
        }

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public async Task<IAsyncCursor<OpLog>> TailCollectionAsync(BsonValue lastInsertDate)
        {
            var options = new FindOptions<OpLog>
            {
                CursorType = CursorType.TailableAwait
            };

            BsonDocument filter = new BsonDocument();

            if (lastInsertDate != null)
                filter = new BsonDocument("ts", new BsonDocument("$gt", lastInsertDate ));

            //var syncCursor = _opLog.Find<OpLog>(filter);

            var cursor = await _opLog.FindAsync(filter, options);
            
            return cursor;
        }

        public List<OpLog> TailCollection(BsonValue lastInsertDate)
        {
            var options = new FindOptions<OpLog>
            {
                CursorType = CursorType.TailableAwait
            };

            BsonDocument filter = new BsonDocument();

            if (lastInsertDate != null)
                filter = new BsonDocument("ts", new BsonDocument("$gt", lastInsertDate));

            filter.Add("ns", new BsonDocument("$ne", String.Empty));

            Console.WriteLine($"filter : {filter.ToString()}");

            var cursor = _opLog.Find<OpLog>(filter);
            try
            {
                return cursor.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"oplog Exception : {ex.Message}");
                return null;
            }
            
        }

        /*
        public void TailCollection()
        {
            // Set lastInsertDate to the smallest value possible
            BsonValue lastInsertDate = BsonMinKey.Value;

            var options = new FindOptions<BsonDocument>
            {
                // Our cursor is a tailable cursor and informs the server to await
                CursorType = CursorType.TailableAwait
            };

            // Initially, we don't have a filter. An empty BsonDocument matches everything.
            BsonDocument filter = new BsonDocument();

            // NOTE: This loops forever. It would be prudent to provide some form of 
            // an escape condition based on your needs; e.g. the user presses a key.
            while (true)
            {
                // Start the cursor and wait for the initial response
                using (var cursor = _opLog.FindSync(filter, options))
                {
                    foreach (var document in cursor.ToEnumerable())
                    {
                        if (document["ns"].ToString() != String.Empty)
                        {
                            // Set the last value we saw 
                            lastInsertDate = document["o"];

                            // Write the document to the console.
                            Console.WriteLine(document.ToString());
                        }
                    }
                }

                // The tailable cursor died so loop through and restart it
                // Now, we want documents that are strictly greater than the last value we saw
                filter = new BsonDocument("$gt", new BsonDocument("insertDate", lastInsertDate));
            }
        }
        */
        /*
        public async Task TailCollectionAsync()
        {
            BsonValue lastId = BsonMinKey.Value;
            while (true)
            {
                var filter = new BsonDocument("$gt", new BsonDocument("_id", lastId));
                var options = new FindOptions<BsonDocument>
                {
                    CursorType = CursorType.TailableAwait
                };

                using (var cursor = await _opLog.FindAsync(filter, options))
                {
                    await cursor.ForEachAsync(document =>
                    {
                        lastId = document["_id"];
                        Console.WriteLine(document.ToJson());
                    });
                }
            }
        }
        */
    }
}
