using SV21T1020526.DataLayers;
using SV21T1020526.DataLayers.SQLServer;
using SV21T1020526.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020526.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        private static readonly IUserAccountDAL shipperAccountDB;
        static UserAccountService()
        {
            String connectionString = Configuration.ConnectionString;
            employeeAccountDB = new EmployeeAccountDAL(connectionString);
            customerAccountDB = new CustomerAccountDAL(connectionString);
            shipperAccountDB = new shipperAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userType,string username, string password)
        {
            if(userType == UserTypes.employee)            
                return employeeAccountDB.Authorize(username, password);
            else if (userType == UserTypes.customer)
            {
                return customerAccountDB.Authorize(username, password);

            }
            else 
            {
                return shipperAccountDB.Authorize(username, password);

            }

            
           
            
        }
        public static int Register(Customer data)
        {
            return customerAccountDB.AddUser(data);
        }
        public static bool UpdateUserPassword(string username, string oldPassword, string newPassword)
        {
            // Kiểm tra tài khoản và mật khẩu cũ từ tất cả các loại tài khoản
            UserAccount? user = Authorize(UserTypes.employee, username, oldPassword)
                                ?? Authorize(UserTypes.customer, username, oldPassword);
                              

            // Nếu không tìm thấy tài khoản hoặc mật khẩu cũ không đúng
            if (user == null)
                return false;

            // Xác định loại người dùng và gọi phương thức cập nhật mật khẩu tương ứng
            if (user.RoleNames.Contains("customer"))
            {
                return customerAccountDB.ChangePassword(username, newPassword);
            }
            else
            {
                return employeeAccountDB.ChangePassword(username, newPassword);
            }
        }








    }
    public enum UserTypes
    {
        employee,
        customer,
        shipper,
        
    }
}
