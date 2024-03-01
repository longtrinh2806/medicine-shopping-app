using Customer.Data.Configurations;
using Customer.Data.MongoCollections;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.DataAccess
{
    public class AppDbContext
    {
        private IMongoClient _client;
        private IMongoDatabase _database;

        public AppDbContext(IOptions<AppDatabaseSetting> options)
        {
            _client = new MongoClient(options.Value.ConnectionString);
            _database = _client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<Customers> Customers => _database.GetCollection<Customers>("customer");
        public IMongoCollection<Address> Addresses => _database.GetCollection<Address>("address");
        public IMongoCollection<Coupon> Coupons => _database.GetCollection<Coupon>("coupon");
        public IMongoCollection<Membership> Memberships => _database.GetCollection<Membership>("membership");
        public IMongoCollection<OrderHistory> OrderHistories => _database.GetCollection<OrderHistory>("order_history");
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
