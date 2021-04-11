using seller_client.models;
using seller_client.models.enums;
using seller_client.services;
using supplier_client.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seller_client.forms
{
    public partial class PurchaseHistoryForm : Form
    {
        public PurchaseHistoryForm()
        {
            InitializeComponent();
        }

        private void PurchaseHistoryForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            updateDataGrid1();
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[0].Selected = true;
                updateForm();
            }
        }


        private void updateForm()
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            if (order.status == ClientOrderStatus.PAID)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
            if (order.status == ClientOrderStatus.CREATED)
            {
                button1.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button3.Enabled = false;
            }
            var ccws = from cc in DataService.db.GetTable<ComputerComponent>()
                       where cc.tradePrice != null
                       join cad in DataService.db.GetTable<ClientAssemblyDetails>() on cc.id equals cad.computerComponentId
                       where cad.clientOrderId == order.id
                       select new ComputerComponentWithCount(cc.id, cc.type, cc.name, cc.description, cc.tradePrice.Value, cc.producer.name, cad.count);
            dataGridView2.DataSource = ccws;
            dataGridView2.Columns["id"].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            updateForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            order.status = ClientOrderStatus.ACCEPT;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            var componentCounts = (from cad in DataService.db.GetTable<ClientAssemblyDetails>()
                                   where cad.clientOrderId == order.id
                                   select cad).ToDictionary(x => x.computerComponentId, x => x.count);
            var componentIds = componentCounts.Keys.ToList();
            var shopStoreCounts = DataService.db.GetTable<ShopStore>()
                .Where(ss => componentIds.Contains(ss.computerComponentId))
                .ToDictionary(ss => ss.computerComponentId, ss => ss.count);
            foreach (KeyValuePair<int, int> entry in componentCounts)
            {
                int shopCount = 0;
                shopStoreCounts.TryGetValue(entry.Key, out shopCount);
                if (shopCount - entry.Value < 0)
                {
                    MessageBox.Show("На складе не хватает компонента с id = " + entry.Key);
                    return;
                }
            }
            order.status = ClientOrderStatus.DONE;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();

            var shopStores = DataService.db.GetTable<ShopStore>()
                .Where(ss => componentIds.Contains(ss.computerComponentId)).ToList();
            foreach (var shopStore in shopStores)
            {
                int orderCount = 0;
                componentCounts.TryGetValue(shopStore.computerComponentId, out orderCount);
                shopStore.count = shopStore.count - orderCount;
            }
            DataService.db.SubmitChanges();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            order.status = ClientOrderStatus.DECLINE;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();
        }

        private void updateDataGrid1()
        {
            var cos = from co in DataService.db.GetTable<ClientOrder>()
                      where co.status != ClientOrderStatus.NEW
                      select co;
            dataGridView1.DataSource = cos;
        }

        private void магазинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            StoreForm form = new StoreForm();
            form.Show();
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void заявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            SupplierApplicationForm form = new SupplierApplicationForm();
            form.Show();
        }

        private void создатьЗаявкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            CreateSupplyAssemblyOrderForm form = new CreateSupplyAssemblyOrderForm();
            form.Show();
        }
    }
}
