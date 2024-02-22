namespace ProductData.MongoCollections
{
    public class DetailedDescription
    {
        public string MoTaSanPham { get; set; }
        public HashSet<string> ThanhPhan { get; set; }
        public string CongDung { get; set; }
        public string TacDungPhu { get; set; }
        public string LuuY { get; set; }
        public string BaoQuan { get; set; }
    }
}