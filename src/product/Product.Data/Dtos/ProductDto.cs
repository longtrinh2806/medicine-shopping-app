using ProductData.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.Dtos
{
    public class ProductDto
    {
        public string TenSanPham { get; set; }
        public string DonViTinh { get; set; }
        public string QuyCach { get; set; }
        public string XuatXu { get; set; }
        public string MoTaNgan { get; set; }
        public string NhaSanXuat { get; set; }
        public long GiaTienGoc { get; set; }
        public long GiaTienHienTai { get; set; }
        public double PhanTramGiamGia { get; set; }
        public int SoLuong { get; set; }
        public List<string> ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string MoTaSanPham { get; set; }
        public Dictionary<string, string> ThanhPhan { get; set; }
        public string CongDung { get; set; }
        public string TacDungPhu { get; set; }
        public string LuuY { get; set; }
        public string BaoQuan { get; set; }
    }
}