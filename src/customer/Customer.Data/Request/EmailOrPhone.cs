using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.Request
{
    public class EmailOrPhone
    {
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
    }
}
