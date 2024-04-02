using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.Templates
{
    public class Price
    {
        public long GiaTienGoc { get; set; }
        public long? GiaTienHienTai { get; set; } = 0L;
        public double? PhanTramGiamGia { get; set; } = 0.0;
    }
}
