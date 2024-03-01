using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.Dtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public Guid ParentId { get; set; } = Guid.Empty;
    }
}
