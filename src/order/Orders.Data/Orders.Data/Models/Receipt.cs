using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Data.Models
{
    public class Receipt
    {
        public Guid Id { get; set; }
        public string NamePharmacy { get; set; } = "NHÀ THUỐC HOÀNG LONG";
        public string PhoneNumber { get; set; } = "1900 0091";
        public string Name { get; set; } = "HÓA ĐƠN BÁN LẺ";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid OrderId { get; set; }
    }
}
