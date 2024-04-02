using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.Dtos
{
    public class AddressDto
    {
        public string SoNha { get; set; }
        public string TenDuong { get; set; }
        public string? Quan { get; set; } = string.Empty;
        public string? Huyen { get; set; } = string.Empty;
        public string? ThanhPho { get; set; } = string.Empty;
        public string? Tinh { get; set; } = string.Empty;
    }
}
