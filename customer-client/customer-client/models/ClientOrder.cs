using customer_client.models;
using customer_client.models.enums;
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
    [Table(Name = "client_order")]
    class ClientOrder
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }
        [DisplayName("Название заказа")]
        [Column(Name = "name")]
        public string name { get; set; }
        [DisplayName("Создано")]
        [Column(Name = "created_at")]
        public DateTime createdAt { get; set; }
        [DisplayName("Цена")]
        [Column(Name = "price")]
        public Decimal? price { get; set; }
        [DisplayName("Статус")]
        [Column(Name = "status", DbType = "varchar(63)")]
        public ClientOrderStatus status { get; set; }

        private EntityRef<Account> _account;

        [Association(Storage = "_account", IsForeignKey = true, ThisKey = "clientId", OtherKey = "id", DeleteRule = null)]
        public Account account { get { return this._account.Entity; } set { this._account.Entity = value; } }

        [Browsable(false)]
        [Column(Name = "client_id")]
        public int clientId { get; set; }

        public ClientOrder(string name, DateTime createdAt, ClientOrderStatus status, Account account)
        {
            this.name = name;
            this.createdAt = createdAt;
            this.status = status;
            this.account = account;
        }

        public ClientOrder(string name, DateTime createdAt, ClientOrderStatus status, int clientId)
        {
            this.name = name;
            this.createdAt = createdAt;
            this.status = status;
            this.clientId = clientId;
        }

        public ClientOrder()
        {
        }
    }
}
