using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Data.Dtos
{
    public class UpdatedCartItemDto
    {
        public int Quantity { get; set; }
        public long Price { get; set; }
    }
}
