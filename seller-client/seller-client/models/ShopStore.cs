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
    [Table(Name = "shop_store")]
    class ShopStore
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }

        [DisplayName("Количество")]
        [Column(Name = "count")]
        public int count { get; set; }

        [DisplayName("Цена продажи")]
        public Decimal? tradePrice { get { return computerComponent.tradePrice; } }

        [DisplayName("Тип компонента")]
        public ComponentType type { get { return computerComponent.type; } }

        [DisplayName("Название")]
        public string name { get { return computerComponent.name; } }

        [DisplayName("Описание")]
        public string description { get { return computerComponent.description; } }


        [Association(Storage = "_computerComponent", IsForeignKey = true, ThisKey = "computerComponentId", OtherKey = "id", DeleteRule = null)]
        public ComputerComponent computerComponent { get { return this._computerComponent.Entity; } set { this._computerComponent.Entity = value; } }

        [Browsable(false)]
        [Column(Name = "computer_component_id")]
        public int computerComponentId { get; set; }

        private EntityRef<ComputerComponent> _computerComponent;

        public ShopStore(int computerComponentId, int count)
        {
            this.computerComponentId = computerComponentId;
            this.count = count;
        }

        public ShopStore()
        {
        }
    }
}
