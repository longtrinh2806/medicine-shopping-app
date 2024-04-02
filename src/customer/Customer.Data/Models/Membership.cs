using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.Models
{
    public class Membership
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int CapDo { get; set; }
        public int DiemTichLuy { get; set; }
        public Customers Customers { get; set; }
    }
}
