using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seller_client.models
{
    [Table(Name = "supply_assembly_details")]
    class SupplyAssemblyDetails
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }

        [Column(Name = "count")]
        public int count { get; set; }

        private EntityRef<SupplyAssemblyOrder> _supplyAssemblyOrder;

        [Association(Storage = "_supplyAssemblyOrder", IsForeignKey = true, ThisKey = "supplyAssemblyOrderId", OtherKey = "id")]
        public SupplyAssemblyOrder supplyAssemblyOrder { get { return this._supplyAssemblyOrder.Entity; } set { this._supplyAssemblyOrder.Entity = value; } }

        private EntityRef<SupplierStore> _supplierStore;

        [Association(Storage = "_supplierStore", IsForeignKey = true, ThisKey = "supplierStoreId", OtherKey = "id")]
        public SupplierStore supplierStore { get { return this._supplierStore.Entity; } set { this._supplierStore.Entity = value; } }

        [Column(Name = "supplier_store_id")]
        public int supplierStoreId { get; set; }

        [Column(Name = "supply_assembly_order_id")]
        public int supplyAssemblyOrderId { get; set; }

        public SupplyAssemblyDetails()
        {
        }

        public SupplyAssemblyDetails(int count, int supplierStoreId, int supplyAssemblyOrderId)
        {
            this.count = count;
            this.supplierStoreId = supplierStoreId;
            this.supplyAssemblyOrderId = supplyAssemblyOrderId;
        }
    }
}
