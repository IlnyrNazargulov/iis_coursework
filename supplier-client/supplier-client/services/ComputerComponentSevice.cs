using supplier_client.models;
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
    class ComputerComponentSevice
    {
        private static ComputerComponentSevice computerComponentSevice;

        private string CREATE_SUPPLIER_STORE = "INSERT INTO supplier_store(computer_component_id, supplier_id, supplier_price, count) " +
            "VALUES(@computer_component_id, @supplier_id, @supplier_price, @count);SELECT SCOPE_IDENTITY()";
        private string CREATE_COMPUTER_COMPONENT = "INSERT INTO computer_component(type, name, description, producer_id) " +
            "VALUES(@type, @name, @description, @producer_id);SELECT SCOPE_IDENTITY()";

        private string GET_ALL_COMPUTER_COMPONENTS = "SELECT cc.id as 'id', cc.type as 'Тип компонета', cc.name as 'Название компонента', cc.description as 'Описание', cc.producer_id as 'ИД производитель', ss.supplier_price as 'Наша цена', ss.count as 'Количество на складе', ss.id as 'ИД компонента на складе'" +
            "FROM computer_component cc " +
            "LEFT JOIN supplier_store ss ON ss.computer_component_id = cc.id and ss.supplier_id = @supplier_id";
        private string GET_ALL_COMPUTER_COMPONENTS_BY_ORDER = "SELECT cc.id, cc.type as 'Тип', cc.name as 'Название', ss.supplier_price as 'Цена', sad.count as 'Количество'" +
            "FROM computer_component cc " +
            "inner join supplier_store ss on ss.computer_component_id = cc.id " +
            "inner join supply_assembly_details sad on ss.id = sad.supplier_store_id " +
            "inner join supply_assembly_order sao on sao.id = sad.supply_assembly_order_id " +
            "where sao.id = @supply_assembly_order_id";



        private string UPDATE_SUPPLIER_STORE = "UPDATE supplier_store set supplier_price = @supplier_price, count = @count where id = @id";
        private string UPDATE_COMPUTER_COMPONENT = "UPDATE computer_component set type = @type, name = @name, description = @description, producer_id = @producer_id  where id = @id";

        public static ComputerComponentSevice getComputerComponentSevice()
        {
            if (computerComponentSevice == null)
            {
                computerComponentSevice = new ComputerComponentSevice();
            }
            return computerComponentSevice;
        }

        public void createComputerComponentWithSupplier(int? componentId, decimal supplierPrice, ComponentType componentType, string name, string description, int producerId, int count)
        {
            if (componentId is int valueOfComponentId)
            {
                createComputerComponentInSupplierStore(valueOfComponentId, supplierPrice, count);
                return;
            }
            int computerComponentId = createComputerComponent(componentType, name, description, producerId);
            createComputerComponentInSupplierStore(computerComponentId, supplierPrice, count);
        }

        private int createComputerComponent(ComponentType componentType, string name, string description, int producerId)
        {
            SqlCommand command = DataService.createSqlCommand(CREATE_COMPUTER_COMPONENT);
            DataService.addParameter("@type", componentType.ToString());
            DataService.addParameter("@description", description);
            DataService.addParameter("@name", name);
            DataService.addParameter("@producer_id", producerId);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        private int createComputerComponentInSupplierStore(int computerComponentId, decimal supplierPrice, int count)
        {
            SqlCommand command = DataService.createSqlCommand(CREATE_SUPPLIER_STORE);
            DataService.addParameter("@supplier_price", supplierPrice);
            DataService.addParameter("@computer_component_id", computerComponentId);
            DataService.addParameter("@count", count);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void updateComputerComponent(int componentId, int? supplierStoreId, decimal supplierPrice, ComponentType componentType,
            string name, string description, int producerId, int count)
        {
            DataService.createTransaction();
            SqlCommand command1 = DataService.createSqlCommand(UPDATE_COMPUTER_COMPONENT);
            DataService.addParameter("@type", componentType.ToString());
            DataService.addParameter("@id", componentId);
            DataService.addParameter("@name", name);
            DataService.addParameter("@description", description);
            DataService.addParameter("@producer_id", producerId);
            command1.ExecuteNonQuery();
            if (supplierStoreId is int valueOfSupplierStoreId)
            {
                SqlCommand command2 = DataService.createSqlCommand(UPDATE_SUPPLIER_STORE);
                DataService.addParameter("@id", valueOfSupplierStoreId);
                DataService.addParameter("@supplier_price", supplierPrice);
                DataService.addParameter("@count", count);
                command2.ExecuteNonQuery();
            }
            else
            {
                createComputerComponentInSupplierStore(componentId, supplierPrice, count);
            }
            DataService.commitTransaction();
        }

        public DataTable getComputerComponentsInDataTable()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_COMPUTER_COMPONENTS);
            DataService.addParameter("@supplier_id", SupplierService.supplierId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public DataTable getComputerComponentsByOrderId(int orderId)
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_COMPUTER_COMPONENTS_BY_ORDER);
            DataService.addParameter("@supply_assembly_order_id", orderId);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }
    }
}
