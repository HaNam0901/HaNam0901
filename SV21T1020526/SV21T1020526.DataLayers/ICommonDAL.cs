namespace SV21T1020526.DataLayers
{
    /// <summary>
    /// định nghĩa các phép xử lý dữ liệu thường dùng trên bảng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {   
        /// <summary>
        /// tìm kiếm và lấy danh sách dữ liêu ( có kiểu T) dưới dạng có phân trang
        /// </summary>
        /// <param name="page">trang cần  hiển thị</param>
        /// <param name="pageSize">số dòng được hiển thị trên 1 trang(bằng 0 nếu không phân trang)</param>
        /// <param name="serchValue">giá trị cần tìm kiếm(chuổi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        List<T> List(int page=1 ,int pageSize =0 , string searchValue="");
        /// <summary>
        /// đếm số lượng dòng dữ liệu tìm kiếm được
        /// </summary>
        /// <param name="searchValue">giá trị tìm kiếm (chuỗi rỗng nếu tìm kiếm trên toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count (string searchValue="");
        /// <summary>
        /// lấy một babnnr ghi dữ liệu vao khóa chính/id(trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// bổ sung bản ghi vào csdl. Hàm trả về id dữ liệu vừa bổ sung (nếu có)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
		/// <summary>
		/// Cập nhật một bản ghi dữ liệu
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Update(T data);
        /// <summary>
        /// Xóa một bản ghi dữ liệu vào giá trị khóa chính/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra bản ghi dữ liệu khóa là id hiện tại đáng có dữ liệu tham chiếu ở bảng khác hay không? 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
