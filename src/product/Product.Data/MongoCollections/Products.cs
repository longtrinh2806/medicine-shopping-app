using MongoDB.Bson.Serialization.Attributes;
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
        public long GiaTien { get; set; }
        public long GiaTienDaGiam { get; set; }
        public int PhanTramGiamGia { get; set; }
        public int SoLuong { get; set; }
        public List<string> ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public DetailedDescription DetailedDescription { get; set; }
    }
}