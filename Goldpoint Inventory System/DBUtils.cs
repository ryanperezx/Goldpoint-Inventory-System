using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Goldpoint_Inventory_System
{
    class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"DESKTOP-QRTQRNN\SQLEXPRESS";
            string database = "GoldpointManagementSystem;" +
                "User ID=admin;" +
                "Password=Deaths123";

            return DBSQLServerUtils.GetDBConnection(datasource, database);
        }
    }
}
