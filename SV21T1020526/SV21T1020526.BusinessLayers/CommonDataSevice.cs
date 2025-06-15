using Azure;
using SV21T1020526.Datalayers;
using SV21T1020526.DataLayers;
using SV21T1020526.DomainModels;
using System.Buffers;
using System.ComponentModel;

namespace SV21T1020526.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
        }
        /// <summary>
        /// Danh sach tat ca tinh thanh 
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {

            return provinceDB.List();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang 
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biêtd số dòng tìm đc </param>
        /// <param name="page">Trang cần hiển thị </param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang </param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch </param>
        /// <returns></returns>

        public static List<Customer> ListOfCustomer(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// lay thong tin cua 1 khach hang dua vao ma 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// bo sung 1 khach hang moi ham tra ve ma cua khac hang dc chon
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// cap nhat thong tin cua khach hang 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// Xoa 1 khach hang 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        /// <summary>
        ///  Kiem tra xem 1 khach hang dang co don hang hay khong 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }


        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của một khách hàng dựa vào mã 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một khách hàng mới hàm trả về mã khách hàng đc chọn 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin của một khách hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// Xóa một khách hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.InUsed(id))
                return false;
            return shipperDB.Delete(id);
        }

        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);

        }



        public static List<Supplier> ListOfSupplier(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của nhà cung cấp dựa vào mã 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers()
        {
            return supplierDB.List();
        }
    public static Supplier? GetSupplier(int id = 0)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một nhà cung cấp 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin của khách hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa một khách hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.InUsed(id))
                return false;
            return supplierDB.Delete(id);
        }

        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }


        public static List<Employee> ListOfEmployee(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của nhân viên dựa vào mã 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id = 0)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một nhân viên 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// cập nhật thông tin của một nhân viên 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// Xóa một nhân viên 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.InUsed(id))
                return false;
            return employeeDB.Delete(id);
        }

        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }

        public static List<Customer> ListOfcustomers()
        {
            return  customerDB.List();
        }
        public static List<Category> ListOfCategorys()
        {
            return categoryDB.List();
        }
        public static List<Shipper> ListOfShipper()
        {
            return shipperDB.List();
        }
        public static List<Category> ListOfCategory(out int rowCount, int oage = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(oage, pageSize, searchValue);
        }

        /// <summary>
        /// lấy thông tin của loại hàng dựa vào mã 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category GetCategory(int id = 0)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một loại hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        public static bool DeleteCategory(int id)
        {
            if (categoryDB.InUsed(id))
                return false;
            return categoryDB.Delete(id);
        }

        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
    }
}
