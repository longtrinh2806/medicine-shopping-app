using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.Dtos
{
    public class ProductUpdatedDto
    {
        public long GiaTien { get; set; }
        public long GiaTienDaGiam { get; set; }
        public int PhanTramGiamGia { get; set; }
        public Guid CategoryId { get; set; } 
    }
}
