using customer_client.models;
using customer_client.models.enums;
using supplier_client.forms;
using supplier_client.models;
using supplier_client.services;
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

namespace customer_client.forms
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

        private void корзинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ShopForm shopForm = new ShopForm();
            shopForm.Show();
        }

        private void магазинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void историяПокупокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            BasketForm basketForm = new BasketForm();
            basketForm.Show();
        }

        private void updateForm()
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            if (order.status == ClientOrderStatus.CREATED)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
            if (order.status == ClientOrderStatus.DONE)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
            if (order.status == ClientOrderStatus.ACCEPT)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
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
            order.status = ClientOrderStatus.CANCEL;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            order.status = ClientOrderStatus.PAID;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClientOrder order = (ClientOrder)dataGridView1.CurrentRow.DataBoundItem;
            order.status = ClientOrderStatus.RECEIVED;
            DataService.db.SubmitChanges();
            updateForm();
            updateDataGrid1();
        }

        private void updateDataGrid1()
        {
            Account account = DataService.currentAccount;
            var cos = from co in DataService.db.GetTable<ClientOrder>()
                      where co.clientId == account.id
                      select co;
            dataGridView1.DataSource = cos;
            dataGridView1.Columns["account"].Visible = false;
        }
    }
}
