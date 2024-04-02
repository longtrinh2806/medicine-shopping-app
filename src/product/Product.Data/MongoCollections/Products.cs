using MongoDB.Bson.Serialization.Attributes;
using Product.Data.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductData.MongoCollections
{
    public class Products
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; }
        public string TenSanPham { get; set; }
        public string DonViTinh { get; set; }
        public string QuyCach { get; set; }
        public string XuatXu { get; set; }
        public string MoTaNgan { get; set; }
        public string NhaSanXuat { get; set; }
        public Price Price { get; set; }
        public List<string> ImageUrl { get; set; }
        public string MoTaSanPham { get; set; }
        public Dictionary<string, string> ThanhPhan { get; set; }
        public string CongDung { get; set; }
        public string TacDungPhu { get; set; }
        public string LuuY { get; set; }
        public string BaoQuan { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid CategoryId { get; set; }
    }
}