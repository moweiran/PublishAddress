using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ConsoleApp1.DbHelper
{
    public static class DBHelper
    {
        public static string ConnectionString = @"Server=.\sql2008r2;Initial Catalog=Address;User ID=sa;Password=lctxw@163.com;Connection Timeout=300;MultipleActiveResultSets=True;";
        public static IDbConnection Connection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }
    }
}
