using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductData.DataAccess;
using ProductData.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.DataAccess
{
    public class AppDbContext
    {
        private IMongoDatabase _database;
        private IMongoClient _client;

        public AppDbContext(IOptions<AppDatabaseSetting> options)
        {
            _client = new MongoClient(options.Value.ConnectionString);
            _database = _client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<Products> Products => _database.GetCollection<Products>("product");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("category");
        public IClientSessionHandle StartSession()
        {
            var session = _client.StartSession();
            return session;
        }

        public void CreateCollectionsIfNotExisted()
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Any(name => name.Equals("product")))
            {
                _database.CreateCollection("product");
            }

            if (!collectionNames.Any(name => name.Equals("category")))
            {
                _database.CreateCollection("category");
            }
            
        }
    }
}
