using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Data.Filters
{
    public class PaginationFilter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 20;

        public PaginationFilter() { }
        public PaginationFilter(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            this.PageSize = pageSize > 20 ? 20 : pageSize;
        }
    }
}
