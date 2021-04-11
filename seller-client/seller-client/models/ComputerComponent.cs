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
    [Table(Name = "computer_component")]
    class ComputerComponent
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [DisplayName("Тип компонента")]
        [Column(Name = "type", DbType = "varchar(63)")]
        public ComponentType type { get; set; }
        [DisplayName("Название")]
        [Column(Name = "name")]
        public string name { get; set; }
        [DisplayName("Описание")]
        [Column(Name = "description")]
        public string description { get; set; }
        [DisplayName("Цена")]
        [Column(Name = "trade_price")]
        public Decimal? tradePrice { get; set; }

        [DisplayName("Производитель")]
        public String producerName { get { return producer.name; } }

        internal EntityRef<Producer> _producer;

        [Browsable(false)]
        [Column(Name = "producer_id")]
        public int producerId { get; set; }

        [Browsable(false)]
        [Association(Storage = "_producer", IsForeignKey = true, OtherKey = "id", ThisKey = "producerId")]
        public Producer producer { get { return this._producer.Entity; } set { this._producer.Entity = value; } }

    }
}
