using System;
using System.Configuration;
using MongoDB.Driver;

namespace Database
{
    public class MongoDBContext
    {
        private static MongoDatabase instance;
        private static object syncRoot = new Object();
        private MongoDBContext() { }
        private static string MongoDbConnectionString = ConfigurationManager.AppSettings["MongoDbConnection"].ToString();
        private static string MongoDatabase = ConfigurationManager.AppSettings["MongoDatabase"].ToString();
        public static MongoDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        //if (instance == null)
                        //{
                        //    MongoClient client = new MongoClient(MongoDbConnectionString);
                        //    MongoServer server=client.GetServer();
                        //    instance = server.GetDatabase(MongoDatabase);
                        //}
                    }
                }
               return instance;
            }
        }
    }
}
