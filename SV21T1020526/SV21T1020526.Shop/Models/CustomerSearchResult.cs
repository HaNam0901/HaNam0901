using SV21T1020526.DomainModels;

namespace SV21T1020526.Shop.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }

    }
}
