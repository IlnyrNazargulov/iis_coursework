using supplier_client.models.enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.services
{
    class SupplyAssemblyOrderService
    {
        private static SupplyAssemblyOrderService assemblyOrderService;

        private string GET_ALL_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID = "SELECT id, status as 'Статус', created_at as 'Дата создания', delivered_at as 'Доставим к' FROM supply_assembly_order WHERE supplier_id = @supplier_id and status not in ('NEW', 'CANCEL')";
        private string GET_ALL_AVAILABLE_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID = "SELECT id, status as 'Статус', created_at as 'Дата создания', delivered_at as 'Доставим к' " +
            "FROM supply_assembly_order WHERE supplier_id = @supplier_id and status in ('CREATED')";
        private string GET_ALL_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID = "SELECT id, status as 'Статус', created_at as 'Дата создания', delivered_at as 'Доставим к' " +
            "FROM supply_assembly_order WHERE supplier_id = @supplier_id and status = 'DECLINE'";
        private string GET_ALL_SENDED_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID = "SELECT id, status as 'Статус', created_at as 'Дата создания', delivered_at as 'Доставим к' " +
            "FROM supply_assembly_order WHERE supplier_id = @supplier_id and status = 'SENT'";

        public string CHECK_AVAILABLE_CC = "SELECT MIN(ss.count - sad.count) FROM supply_assembly_order sao " +
            "INNER JOIN supply_assembly_details sad ON sad.supply_assembly_order_id = sao.id " +
            "INNER JOIN supplier_store ss ON ss.id = sad.supplier_store_id " +
            "WHERE sao.id = @id";

        public string UPDATE_SUPPLIER_STORE = "UPDATE supplier_store set count = count - (select count from supply_assembly_details where supplier_store_id = supplier_store.id and supply_assembly_order_id = @supply_assembly_order_id) where id = (select supplier_store_id from supply_assembly_details where supplier_store_id = supplier_store.id and supply_assembly_order_id = @supply_assembly_order_id)";

        private string UPDATE_STATUS_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER = "UPDATE supply_assembly_order SET status = @status WHERE id = @id";
        private string UPDATE_STATUS_AND_DELIVERED_AT_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER = "UPDATE supply_assembly_order SET status = @status, delivered_at = @delivered_at WHERE id = @id";


        public static SupplyAssemblyOrderService getSupplyAssemblyOrderService()
        {
            if (assemblyOrderService == null)
            {
                assemblyOrderService = new SupplyAssemblyOrderService();
            }
            return assemblyOrderService;
        }

        public DataTable getSupplyAssemblyOrdersInDataTable()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public DataTable getAvailableSupplyAssemblyOrdersInDataTable()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_AVAILABLE_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public DataTable getInaccessibleSupplyAssemblyOrdersInDataTable()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public DataTable getSendedSupplyAssemblyOrdersInDataTable()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_SENDED_SUPPLY_ASSEMBLY_ORDER_BY_SUPPLIER_ID);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public void updateStatusSupplyAssemblyOrders(int id, SupplyAssemblyOrderStatus status)
        {
            SqlCommand command = DataService.createSqlCommand(UPDATE_STATUS_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER);
            DataService.addParameter("@status", status.ToString());
            DataService.addParameter("@id", id);
            command.ExecuteNonQuery();
        }

        public void updateStatusAndDeliveredAtSupplyAssemblyOrders(int id, SupplyAssemblyOrderStatus status, DateTime deliveredAt)
        {
            SqlCommand command = DataService.createSqlCommand(UPDATE_STATUS_AND_DELIVERED_AT_INACCESSIBLE_SUPPLY_ASSEMBLY_ORDER);
            DataService.addParameter("@status", status.ToString());
            DataService.addParameter("@delivered_at", deliveredAt);
            DataService.addParameter("@id", id);
            command.ExecuteNonQuery();
        }

        public int checkOrder(int supplyAssemblyOrderId)
        {
            SqlCommand command = DataService.createSqlCommand(CHECK_AVAILABLE_CC);
            DataService.addParameter("@id", supplyAssemblyOrderId);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void updateSupplierStore(int supplyAssemblyOrderId)
        {
            SqlCommand command = DataService.createSqlCommand(UPDATE_SUPPLIER_STORE);
            DataService.addParameter("@supply_assembly_order_id", supplyAssemblyOrderId);
            command.ExecuteNonQuery();
        }
    }
}
