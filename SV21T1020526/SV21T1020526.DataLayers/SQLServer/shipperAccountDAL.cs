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
    public class shipperAccountDAL : BaseDAL, IUserAccountDAL
    {
        public shipperAccountDAL(string connectionString) : base(connectionString)
        {
        }

		public int AddUser(Customer data)
		{
			throw new NotImplementedException();
		}

		public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT 
                                ShipperID AS UserId, 
                                ShipperName AS UserName,                                                                                        
                                RoleNames 
                            FROM Shippers 
                            WHERE ShipperName = @ShipperName AND Phone = @Phone";

                var parameters = new
                {
                    ShipperName = username,
                    Phone = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
