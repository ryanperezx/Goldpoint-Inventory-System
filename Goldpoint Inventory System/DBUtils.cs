﻿using System;
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
            /*
            string datasource = @"192.168.1.171\SQLEXPRESS";
            string database = "GoldpointManagementSystem;" +
                "User ID=sa;" +
                "Password=goldpoint";
            */
            
            string datasource = @"192.168.0.24,1433";
            string database = "GoldpointManagementSystem;" + 
                "User ID=sa;" + 
                "Password=g0UViHqHq8Ac";
            
            return DBSQLServerUtils.GetDBConnection(datasource, database);
        }
    }
}
