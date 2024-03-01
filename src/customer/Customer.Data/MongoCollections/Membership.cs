using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.MongoCollections
{
    public class Membership
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; }
        public int CapDo { get; set; }
        public int DiemTichLuy { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid CustomerId { get; set; }
    }
}
