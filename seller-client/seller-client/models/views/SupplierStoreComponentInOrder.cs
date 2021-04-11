using seller_client.models.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seller_client.models
{
    class SupplierStoreComponentInOrder
    {
        [DisplayName("ID компонента")]
        public int id { get; set; }
        [DisplayName("Тип компонента")]
        public ComponentType type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Цена поставщика")]
        public Decimal price { get; set; }
        [DisplayName("Количество")]
        public int count { get; set; }

        [Browsable(false)]
        public int supplyAssemblyDetailsId { get; set; }

        public SupplierStoreComponentInOrder(int id, ComponentType type, string name, decimal price, int supplyAssemblyDetailsId, int count)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.price = price;
            this.supplyAssemblyDetailsId = supplyAssemblyDetailsId;
            this.count = count;
        }
    }
}
