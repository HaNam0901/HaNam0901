using SV21T1020526.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020526.DataLayers
{
    public interface IOrderDAL
    {
        // Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang 
        IList<Order> List(int page = 1, int pageSize = 0,
        int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");
        
        // Đếm số lượng đơn hàng thỏa điều kiện tìm kiếm 
        int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");
        
        Order? Get(int orderID);
        
        int Add(Order data);
        
        bool Update(Order data);
        
        bool Delete(int orderID);
         
        IList<OrderDetail> ListDetails(int orderID);
        OrderDetail? GetDetail(int orderID, int productID);

        /// Thêm mặt hàng được bán trong đơn hàng) theo nguyên tắc: 
        /// - Nếu mặt hàng chưa có trong chi tiết đơn hàng thì bổ sung 
        /// - Nếu mặt hàng đã có trong chi tiết đơn hàng thì cập nhật lại số lượng và giá bán    
        bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice); 

        bool DeleteDetail(int orderID, int productID);
		IList<Order> ListOrder(int CustomerId);
	}

}
