using supplier_client.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.services
{
    class SupplierService
    {
        public static int supplierId;

        private static SupplierService supplierService;

        private string GET_SUPPLIER_BY_NAME = "SELECT * FROM supplier where name = @name and is_removed = 0";
        private string GET_SUPPLIER_BY_ACCOUNT_ID = "SELECT * FROM supplier where account_id = @account_id";

        private string CREATE_SUPPLIER = "INSERT INTO supplier(name, account_id, description) VALUES(@name, @account_id, @description)";

        private static string UPDATE_SUPPLIER_INFO = "UPDATE supplier SET name = @name, description = @description " +
            "WHERE id = @id";

        private static string UPDATE_ACCOUNT_IS_REMOVED = "UPDATE supplier SET is_removed = 1 " +
            "WHERE id = @id";

        public static SupplierService getSupplierService()
        {
            if (supplierService == null)
            {
                supplierService = new SupplierService();
                return supplierService;
            }
            return supplierService;
        }

        public void updateSupplier(
            String nameSupplier,
            String descriptionSupplier
            )
        {
            SqlCommand command = DataService.createSqlCommand(UPDATE_SUPPLIER_INFO);
            DataService.addParameter("@name", nameSupplier);
            DataService.addParameter("@description", descriptionSupplier);
            DataService.addParameter("@id", SupplierService.supplierId);
            command.ExecuteNonQuery();
        }

        public void deleteSupplier()
        {
            SqlCommand command = DataService.createSqlCommand(UPDATE_ACCOUNT_IS_REMOVED);
            DataService.addParameter("@id", SupplierService.supplierId);
            command.ExecuteNonQuery();
        }

        public Supplier getSupplierByName(string nameParam)
        {
            SqlCommand command = DataService.createSqlCommand(GET_SUPPLIER_BY_NAME);
            DataService.addParameter("@name", nameParam);
            SqlDataReader reader = command.ExecuteReader();
            Supplier supplier = null;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["id"]);
                    string name = reader["name"].ToString();
                    string description = reader["description"].ToString();
                    supplier = new Supplier(id, name, description);
                }
            }
            else
            {
                reader.Close();
                return null;
            }
            reader.Close();
            return supplier;
        }

        public int createSupplier(String nameParam, String descriptionParam, int accountIdParam)
        {
            SqlCommand command = DataService.createSqlCommand(CREATE_SUPPLIER);
            DataService.addParameter("@name", nameParam);
            DataService.addParameter("@description", descriptionParam);
            DataService.addParameter("@account_id", accountIdParam);
            return command.ExecuteNonQuery();
        }

        public Supplier getSupplierByAccountId(int accountId)
        {
            SqlCommand command = DataService.createSqlCommand(GET_SUPPLIER_BY_ACCOUNT_ID);
            DataService.addParameter("@account_id", accountId);

            SqlDataReader reader = command.ExecuteReader();
            Supplier supplier = null;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["id"]);
                    string name = reader["name"].ToString();
                    string description = reader["description"].ToString();
                    supplier = new Supplier(id, name, description);
                }
            }
            else
            {
                reader.Close();
                return null;
            }
            reader.Close();
            return supplier;
        }
    }
}
