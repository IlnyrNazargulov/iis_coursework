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
    class ComputerComponentWithSupplier
    {
        public int id { get; set; }
        [DisplayName("Тип компонента")]
        public ComponentType type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Описание")]
        public string description { get; set; }
        [DisplayName("Цена поставщика")]
        public Decimal price { get; set; }

        [DisplayName("Производитель")]
        public String producerName { get; set; }

        [DisplayName("Поставщик")]
        public String supplierName { get { return supplier.name; } }

        [Browsable(false)]
        [DisplayName("Поставщик")]
        public Supplier supplier { get; set; }

        [Browsable(false)]
        public int supplierStoreId { get; set; }

        public ComputerComponentWithSupplier(int id, ComponentType type, string name, string description, decimal price, string producerName, Supplier supplier, int supplierStoreId)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.description = description;
            this.price = price;
            this.producerName = producerName;
            this.supplier = supplier;
            this.supplierStoreId = supplierStoreId;
        }
    }
}
