using customer_client.models;
using customer_client.models.enums;
using supplier_client.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
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

        public static DataContext db;
        public static Account currentAccount;
        public static ClientOrder order;

        static DataService()
        {
            db = new DataContext(connectionString);
            DataService.db.DeferredLoadingEnabled = false;

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<ComputerComponent>(сс => сс.producer);
            options.LoadWith<ClientOrder>(co => co.account);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.clientOrder);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.computerComponent);
            db.LoadOptions = options;

            db.Connection.Open();
        }

        public static ClientOrder getOrCreateClientOrder()
        {
            var cos = from co in DataService.db.GetTable<ClientOrder>()
                      where co.status == ClientOrderStatus.NEW &&
                          co.account == DataService.currentAccount
                      select co;
            if (cos.Count() != 0)
            {
                order = cos.Single();
            }
            else
            {
                DateTime now = DateTime.Now;
                string name = "Заказ_от_" + now.ToShortDateString();
                order = new ClientOrder(name, now, ClientOrderStatus.NEW, DataService.currentAccount.id);
                DataService.db.GetTable<ClientOrder>().InsertOnSubmit(order);
                DataService.db.SubmitChanges();
            }
            return order;
        }

        public static void refreshAll()
        {

            db.Connection.Close();
            db = new DataContext(connectionString);
            DataService.db.DeferredLoadingEnabled = true;

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<ComputerComponent>(сс => сс.producer);
            options.LoadWith<ClientOrder>(co => co.account);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.clientOrder);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.computerComponent);
            db.LoadOptions = options;

            db.Connection.Open();
        }
    }
}
