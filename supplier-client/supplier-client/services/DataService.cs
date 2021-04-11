using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.services
{
    class DataService
    {
        private static string connectionString = ConfigurationManager
                .ConnectionStrings["computer_store_coursework"]
                .ConnectionString;

        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand;
        private static SqlTransaction sqlTransaction;

        static DataService() {
          sqlConnection =  getConnection();
        }

        public static SqlConnection getConnection()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
            }
            return sqlConnection;
        }

        public static SqlTransaction createTransaction()
        {
            sqlTransaction = sqlConnection.BeginTransaction();
            return sqlTransaction;
        }

        public static SqlCommand createSqlCommand(string command)
        {
            sqlCommand = new SqlCommand(command, sqlConnection);
            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }
            return sqlCommand;
        }

        public static void addParameter(String nameParam, Object value)
        {
            SqlParameter param = new SqlParameter(nameParam, value);
            sqlCommand.Parameters.Add(param);
        }

        public static void commitTransaction()
        {
            sqlTransaction.Commit();
            sqlTransaction = null;
        }

        public static void rollbackTransaction()
        {
            sqlTransaction.Rollback();
            sqlTransaction = null;
        }
    }
}
