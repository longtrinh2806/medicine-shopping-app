namespace Customer.Data.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public string SoNha { get; set; }
        public string TenDuong { get; set; }
        public string? Quan { get; set; } = string.Empty;
        public string? Huyen { get; set; } = string.Empty;
        public string? ThanhPho { get; set; } = string.Empty;
        public string? Tinh { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Customers Customers { get; set; }
    }
}