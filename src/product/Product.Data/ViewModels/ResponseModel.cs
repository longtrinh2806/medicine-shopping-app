using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.ViewModels
{
    public class ResponseModel
    {
        public object? Data { get; set; }
        public bool Succeeded { get; set; } = true;
        public string? Message { get; set; }
    }
}
