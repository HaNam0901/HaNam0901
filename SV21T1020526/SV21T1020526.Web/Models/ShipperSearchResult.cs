using SV21T1020526.DomainModels;

namespace SV21T1020526.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }

    }
}
