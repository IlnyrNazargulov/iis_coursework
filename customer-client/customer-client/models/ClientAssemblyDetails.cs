using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models
{
    [Table(Name = "client_assembly_details")]
    class ClientAssemblyDetails
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }

        [Column(Name = "count")]
        public int count { get; set; }

        private EntityRef<ClientOrder> _clientOrder;

        [Association(Storage = "_clientOrder", IsForeignKey = true, ThisKey = "clientOrderId", OtherKey = "id", DeleteRule = null)]
        public ClientOrder clientOrder { get { return this._clientOrder.Entity; } set { this._clientOrder.Entity = value; } }

        private EntityRef<ComputerComponent> _computerComponent;

        [Association(Storage = "_computerComponent", IsForeignKey = true, ThisKey = "computerComponentId", OtherKey = "id", DeleteRule = null)]
        public ComputerComponent computerComponent { get { return this._computerComponent.Entity; } set { this._computerComponent.Entity = value; } }

        [Column(Name = "computer_component_id")]
        public int computerComponentId { get; set; }


        [Column(Name = "client_order_id")]
        public int clientOrderId { get; set; }

        public ClientAssemblyDetails(ClientOrder clientOrder, ComputerComponent computerComponent, int count)
        {
            this.clientOrder = clientOrder;
            this.computerComponent = computerComponent;
            this.count = count;
        }

        public ClientAssemblyDetails()
        {
        }

        public ClientAssemblyDetails(int count, int computerComponentId, int clientOrderId)
        {
            this.count = count;
            this.computerComponentId = computerComponentId;
            this.clientOrderId = clientOrderId;
        }
    }
}
