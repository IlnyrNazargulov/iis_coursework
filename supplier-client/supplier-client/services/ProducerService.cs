using supplier_client.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.services
{
    class ProducerService
    {
        private static ProducerService producerService;
        private string GET_ALL_PRODUCERS = "SELECT id, name FROM producer";

        public static ProducerService getProducerService()
        {
            if (producerService == null)
            {
                producerService = new ProducerService();
                return producerService;
            }
            return producerService;
        }

        public List<Producer> getProducers()
        {
            SqlCommand command = DataService.createSqlCommand(GET_ALL_PRODUCERS);
            SqlDataReader reader = command.ExecuteReader();
            List<Producer> producers = new List<Producer>();
            while (reader.Read())
            {
                string name = reader["name"].ToString();
                int id = Convert.ToInt32(reader["id"]);
                producers.Add(new Producer(id, name));
            }
            reader.Close();
            return producers;
        }
    }
}
