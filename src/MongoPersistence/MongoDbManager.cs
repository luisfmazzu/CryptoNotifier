using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MongoPersistence
{
    public class MongoDbManager
    {
        public const string MongoConnectionString = "mongoDbConnectionString";

        //Only one database for now. 
        private readonly IMongoDatabase _database;
        

        public MongoDbManager(IConfiguration configuration)
        {
            _database = InitializeDatabase(configuration[MongoConnectionString]);
        }

        private IMongoDatabase InitializeDatabase(string connectionString)
        {
            var connection = MongoUrl.Create(connectionString);
            var settings = MongoClientSettings.FromUrl(connection);
            var client = new MongoClient(settings);

            return client.GetDatabase(connection.DatabaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            try
            {
                return _database.GetCollection<T>(collectionName);
            }
            catch(Exception)
            {
                //TODO - Log exception
                return null;
            }
        }


    }
}
