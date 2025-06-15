using SV21T1020526.DomainModels;

namespace SV21T1020526.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }

    }
}
