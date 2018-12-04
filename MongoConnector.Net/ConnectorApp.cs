using MongoConnector.Net.Clients;
using MongoConnector.Net.Model;
using MongoConnector.Net.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoConnector.Net
{
    public class ConnectorApp
    {
        private MongoOpLogManager _mongoOpLogManager = new MongoOpLogManager();
        private ConnectorIntermediary _connectorIntermediary;

        public void Init()
        {
            InitSetting();

            _connectorIntermediary = new ConnectorIntermediary();

            _connectorIntermediary.Initlization();
        }

        private void InitSetting()
        {
            var database = MongoDriverClient.GetDatabase("SETTING");
        }

        public void Run()
        {
            var dateTime = DateTime.Now;
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var unixDateTime = dateTimeOffset.ToUnixTimeSeconds();

            Console.WriteLine($"Run Start lastInsertDate : {dateTimeOffset.ToLocalTime().ToString()}");

            BsonValue lastInsertDate = new BsonTimestamp((int)unixDateTime, 1);

            while (true)
            {
                BsonValue lastId = BsonMinKey.Value;

                List<OpLog> oplogs = _mongoOpLogManager.TailCollection(lastInsertDate);

                if(oplogs != null && oplogs.Count > 0)
                {
                    Console.WriteLine($"==============================================\n");
                    Console.WriteLine($"oplogs.Count : {oplogs.Count}");
                    lastInsertDate = _connectorIntermediary.Run(oplogs);
                    Console.WriteLine($"==============================================\n");

                }
                else
                {
                    Console.WriteLine($"oplogs is empty");

                    dateTime = DateTime.Now;
                    dateTimeOffset = new DateTimeOffset(dateTime);
                    unixDateTime = dateTimeOffset.ToUnixTimeSeconds();

                    lastInsertDate = new BsonTimestamp((int)unixDateTime, 1);
                }
                Console.WriteLine($"lastInsertDate : {lastInsertDate.ToString()}");
                Console.WriteLine($"current Time : {DateTime.Now.ToString()}");
                Console.WriteLine("\n");

                System.Threading.Thread.Sleep(3000);
            }
        }

        public void Exit()
        {

        }
    }
}
