using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Goldpoint_Inventory_System
{
    class DBSQLServerUtils
    {
        public static SqlConnection GetDBConnection(string datasource, string database)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog=" + database;
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
