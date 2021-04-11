using customer_client.models.enums;
using supplier_client.forms;
using supplier_client.models;
using supplier_client.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace customer_client.forms
{
    public partial class BasketForm : Form
    {
        public BasketForm()
        {
            InitializeComponent();
        }

        private void BasketForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            updateForm();
        }

        private void updateTotal(IQueryable<ComputerComponentWithCount> ccws)
        {
            decimal total = 0;
            foreach (ComputerComponentWithCount ccw in ccws)
            {
                total = total + ccw.countInOrder * ccw.tradePrice;
            }
            label3.Text = total.ToString();
        }

        private void магазинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void корзинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ShopForm shopForm = new ShopForm();
            shopForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int componentId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            ClientOrder order = DataService.order;
            DataService.db.ExecuteCommand("delete from client_assembly_details where client_order_id = {0} and " +
            "computer_component_id = {1}", order.id, componentId);
            updateForm();
        }

        private void updateForm()
        {
            ClientOrder order = DataService.order;
            if (order.price <= 0)
            {
                button3.Enabled = false;
            }
            else
            {
                button3.Enabled = true;
            }
            var ccws = from cc in DataService.db.GetTable<ComputerComponent>()
                       where cc.tradePrice != null
                       join cad in DataService.db.GetTable<ClientAssemblyDetails>() on cc.id equals cad.computerComponentId
                       where cad.clientOrderId == order.id
                       select new ComputerComponentWithCount(cc.id, cc.type, cc.name, cc.description, cc.tradePrice.Value, cc.producer.name, cad.count);
            dataGridView1.DataSource = ccws;
            if (ccws.Count() == 0)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
            updateTotal(ccws);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientOrder order = DataService.order;
            DataService.db.ExecuteCommand("delete from client_assembly_details where client_order_id = {0}", order.id);
            updateForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Корзина пуста.");
                return;
            }
            ClientOrder order = DataService.order;
            DataService.db.ExecuteCommand("update client_order set status = {0} where id  = {1}",
                ClientOrderStatus.CREATED.ToString(), order.id);
            DataService.getOrCreateClientOrder();
            updateForm();
        }

        private void историяПокупокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            PurchaseHistoryForm purchaseHistoryForm = new PurchaseHistoryForm();
            purchaseHistoryForm.Show();
        }
    }
}
