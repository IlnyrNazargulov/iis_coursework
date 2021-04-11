using supplier_client.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models
{
    class ComputerComponent
    {
        public int id { get; set; }
        public ComponentType type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal supplierPrice { get; set; }
        public int supplierStoreId { get; set; }
        public int count { get; set; }
        public int producerId { get; set; }
    }
}
