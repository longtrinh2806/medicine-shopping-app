using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_order_contract
{
    public class CheckProductInventory
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
