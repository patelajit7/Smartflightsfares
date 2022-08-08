using System;
using System.Configuration;
using MongoDB.Driver;

namespace Database
{
    public class MongoDBContext
    {
        private static IMongoDatabase instance;
        private static object syncRoot = new Object();
        private MongoDBContext() { }
        private static string MongoDbConnectionString = ConfigurationManager.AppSettings["MongoDbConnection"].ToString();
        private static string MongoDatabase = ConfigurationManager.AppSettings["MongoDatabase"].ToString();
        public static IMongoDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            MongoClient client = new MongoClient(MongoDbConnectionString);
                            instance = client.GetDatabase(MongoDatabase);
                        }
                    }
                }
                return instance;
            }
        }
    }
}
