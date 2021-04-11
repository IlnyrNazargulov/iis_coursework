using seller_client.models;
using seller_client.models.enums;
using supplier_client.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seller_client.services
{
    class DataService
    {
        private static string connectionString = ConfigurationManager
                .ConnectionStrings["computer_store_coursework"]
                .ConnectionString;

        public static DataContext db;
        public static Account currentAccount;

        static DataService()
        {
            db = new DataContext(connectionString);
            DataService.db.DeferredLoadingEnabled = true;

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<ComputerComponent>(сс => сс.producer);
            options.LoadWith<ShopStore>(ss => ss.computerComponent);
            options.LoadWith<ClientOrder>(co => co.account);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.clientOrder);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.computerComponent);
            options.LoadWith<SupplyAssemblyOrder>(sad => sad.supplier);

            db.LoadOptions = options;
        }

        public static void refreshAll()
        {
            db.Connection.Close();
            db = new DataContext(connectionString);
            DataService.db.DeferredLoadingEnabled = true;

            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<ComputerComponent>(сс => сс.producer);
            options.LoadWith<ShopStore>(ss => ss.computerComponent);
            options.LoadWith<ClientOrder>(co => co.account);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.clientOrder);
            options.LoadWith<ClientAssemblyDetails>(cad => cad.computerComponent);
            options.LoadWith<SupplyAssemblyOrder>(sad => sad.supplier);

            db.LoadOptions = options;
            db.Connection.Open();
        }
    }
}
