using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Product.Data.Configurations;
using Product.Data.MongoCollections;
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

            var productsCollection = _database.GetCollection<Products>("product");
            var keys = Builders<Products>.IndexKeys
                .Text(p => p.TenSanPham)
                .Text(p => p.MoTaNgan)
                .Text(p => p.CongDung);
            var indexModel = new CreateIndexModel<Products>(keys);
            productsCollection.Indexes.CreateOne(indexModel);
        }

        public IMongoCollection<Products> Products => _database.GetCollection<Products>("product");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("category");
        public IMongoCollection<Inventory> Inventories => _database.GetCollection<Inventory>("inventory");
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
            if (!collectionNames.Any(name => name.Equals("inventory")))
            {
                _database.CreateCollection("inventory");
            }
        }
    }
}
