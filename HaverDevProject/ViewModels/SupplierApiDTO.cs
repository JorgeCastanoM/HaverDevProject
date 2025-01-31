namespace HaverDevProject.ViewModels
{
    public class SupplierApiDTO
    {       
        public int SupplierId { get; set; }
        public string SummarySuplier => $"{SupplierCode} - {SupplierName}";
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public bool SupplierStatus { get; set; }
    }
}
