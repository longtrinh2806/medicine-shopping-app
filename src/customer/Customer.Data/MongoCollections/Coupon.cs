using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.MongoCollections
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime ExpirationAt { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid CustomerId { get; set; }
    }
}
