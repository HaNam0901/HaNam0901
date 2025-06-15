using Microsoft.SqlServer.Server;
using SV21T1020526.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020526.DataLayers
{
    public interface IUserAccountDAL
    {
        public UserAccount? Authorize(string username, string password);

        bool ChangePassword(string username, string password);
        int AddUser(Customer data);

	}
    
	}
