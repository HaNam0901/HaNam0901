using Dapper;
using SV21T1020526.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020526.DataLayers.SQLServer
{
    public  class CustomerAccountDAL:BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public bool ChangePassword(string username, string password)
        {
            using (var connection = OpenConnection())
            {
                // Query cập nhật mật khẩu
                var sql = @"
                    UPDATE Customers
                    SET Password = @Password
                    WHERE Email = @Email";

                var parameters = new
                {
                    Email = username,
                    Password = password
                };

                // Thực thi câu lệnh và kiểm tra kết quả
                int rowsAffected = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();

                return rowsAffected > 0; // Trả về true nếu có dòng bị ảnh hưởng
            }
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT 
                                CustomerID AS UserId, 
                                Email AS UserName, 
                                CustomerName AS DisplayName, 
                                
                                RoleNames 
                            FROM Customers 
                            WHERE Email = @Email AND Password = @Password";

                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
		public int AddUser(Customer data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"if exists(select * from Customers where Email = @Email)
                        select -1
                           else
                            begin
                          insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email,Password,RoleNames)
                          values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password,'customer');
                          select CAST(SCOPE_IDENTITY() as int);
                            end";
				var parameters = new
				{
					CustomerName = data.CustomerName ?? "",
					ContactName = data.ContactName ?? "",
					Province = data.Province ?? "",
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
					Password = data.Password,



				};
				//Thực thi câu lệnh sql
				id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
				connection.Close();
			}
			return id;
		}
	}
}
