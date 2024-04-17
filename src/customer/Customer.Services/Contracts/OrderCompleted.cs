using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_order_contract
{
    public class OrderCompleted
    {
        public Guid CustomerId { get; set; }
        public int DiemTichLuy { get; set; }
    }
}
