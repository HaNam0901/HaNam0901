namespace SV21T1020526.Web.Models
{
    /// <summary>
    /// lưu giữ thông tin đầu vào và sử dụng cho chức năng tìm kiếm và hiển thị giữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
       /// <summary>
       /// trang cần hiển thị
       /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// số dòng hiển thị mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// chuỗi giá trị cần tìm
        /// </summary>
        public string SearchValue { get; set; } = "";

    }
}
