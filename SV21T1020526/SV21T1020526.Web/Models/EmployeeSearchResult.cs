using SV21T1020526.DomainModels;

namespace SV21T1020526.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }

    }
}
