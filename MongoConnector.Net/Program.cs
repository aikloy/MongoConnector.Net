using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoConnector.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectorApp opLogApp = new ConnectorApp();

            opLogApp.Init();

            opLogApp.Run();

            opLogApp.Exit();
        }
    }
}
