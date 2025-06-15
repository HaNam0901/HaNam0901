using Microsoft.Data.SqlClient;

namespace SV21T1020526.DataLayers.SQLServer
{
    /// <summary>
    /// lớp cơ sở (lớp cha) của các lớp cài đặt các phép xử lý dữ liệu trên sql server
    /// </summary>
    public class BaseDAL
    {
        /// <summary>
        /// chuỗi tham số kết nối CSDL SQL Server
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở kết nối đến CSDL(SQL Server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection=new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
