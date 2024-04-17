using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_customer_contract
{
    public class UserCreated
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}
