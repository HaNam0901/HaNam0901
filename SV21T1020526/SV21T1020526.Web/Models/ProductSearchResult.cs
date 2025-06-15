using SV21T1020526.DomainModels;

namespace SV21T1020526.Web.Models
{
    public class ProductSearchResult : PaginationSearchResult
    {
        public List<Product> Data { get; set; } = new List<Product>();
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; internal set; }
        public decimal MaxPrice { get; internal set; }
    }
}
