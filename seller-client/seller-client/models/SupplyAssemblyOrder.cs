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
    [Table(Name = "supply_assembly_order")]
    class SupplyAssemblyOrder
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }
        [DisplayName("Название")]
        [Column(Name = "name")]
        public string name { get; set; }
        [DisplayName("Создана")]
        [Column(Name = "created_at")]
        public DateTime createdAt { get; set; }
        [DisplayName("Сумма")]
        [Column(Name = "price")]
        public Decimal? price { get; set; }
        [DisplayName("Статус")]
        [Column(Name = "status", DbType = "varchar(63)")]
        public SupplyAssemblyOrderStatus status { get; set; }

        private EntityRef<Supplier> _supplier;

        [DisplayName("Поставщик")]
        public string supplierName { get { return supplier.name; } }

        [Browsable(false)]
        [Association(Storage = "_supplier", IsForeignKey = true, ThisKey = "supplierId", OtherKey = "id")]
        public Supplier supplier { get { return this._supplier.Entity; } set { this._supplier.Entity = value; } }

        [Browsable(false)]
        [Column(Name = "supplier_id")]
        public int supplierId { get; set; }

        public SupplyAssemblyOrder()
        {
        }

        public SupplyAssemblyOrder(string name, DateTime createdAt, SupplyAssemblyOrderStatus status, Supplier supplier)
        {
            this.name = name;
            this.createdAt = createdAt;
            this.status = status;
            this.supplier = supplier;
        }
    }
}
