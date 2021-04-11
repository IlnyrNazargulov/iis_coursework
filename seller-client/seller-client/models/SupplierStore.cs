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
    [Table(Name = "supplier_store")]
    class SupplierStore
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }

        [Association(Storage = "_computerComponent", IsForeignKey = true, ThisKey = "computerComponentId", OtherKey = "id", DeleteRule = null)]
        public ComputerComponent computerComponent { get { return this._computerComponent.Entity; } set { this._computerComponent.Entity = value; } }

        [Column(Name = "computer_component_id")]
        public int computerComponentId { get; set; }

        private EntityRef<ComputerComponent> _computerComponent;

        private EntityRef<Supplier> _supplier;

        [Association(Storage = "_supplier", IsForeignKey = true, ThisKey = "supplierId", OtherKey = "id")]
        public Supplier supplier { get { return this._supplier.Entity; } set { this._supplier.Entity = value; } }

        [Browsable(false)]
        [Column(Name = "supplier_id")]
        public int supplierId { get; set; }

        [Column(Name = "count")]
        public int count { get; set; }

        [DisplayName("Цена поставщика")]
        [Column(Name = "supplier_price")]
        public Decimal supplierPrice { get; set; }
    }
}
