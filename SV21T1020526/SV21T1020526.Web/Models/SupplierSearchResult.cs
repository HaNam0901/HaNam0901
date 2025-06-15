using SV21T1020526.DomainModels;

namespace SV21T1020526.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }

    }
}
