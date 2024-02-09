using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Policy;

namespace DataLibrary.DataAccess
{
    public static class SqlDataAccess
    {
        // returns the connectin string from MVCApp/Web.config
        public static string GetConnectionString(string connectionName = "Database")
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        // takes a sql query as parameter and return a generic list
        // methode will be used in the Business Logic Layer
        public static List<T> LoadData<T>(string sql)
        {
            using (IDbConnection connection = new SqlConnection(GetConnectionString()))
            {
                return connection.Query<T>(sql).ToList();
            }
        }

        // takes a sql query & a generic data object as parameter and saves it in the db
        // methode will be used in the Business Logic Layer
        public static int SaveData<T>(string sql, T data)
        {
            using (IDbConnection connection = new SqlConnection(GetConnectionString()))
            {
                return connection.Execute(sql, data);
            }
        }
    }
}
