using supplier_client.models.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models
{
    class ComputerComponentWithCount
    {
        public int id { get; set; }
        [DisplayName("Тип компонента")]
        public ComponentType type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Описание")]
        public string description { get; set; }
        [DisplayName("Цена")]
        public Decimal tradePrice { get; set; }

        [DisplayName("Производитель")]
        public String producerName { get; set; }

        [DisplayName("Количество в заказе")]
        public int countInOrder { get; set; }

        public ComputerComponentWithCount(int id, ComponentType type, string name, string description, decimal tradePrice, string producerName, int countInOrder)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.description = description;
            this.tradePrice = tradePrice;
            this.producerName = producerName;
            this.countInOrder = countInOrder;
        }
    }
}
