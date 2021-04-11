using seller_client.models;
using seller_client.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seller_client.forms
{
    public partial class StoreForm : Form
    {
        public StoreForm()
        {
            InitializeComponent();
        }

        private void главнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void StoreForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            var sss = from ss in DataService.db.GetTable<ShopStore>()
                      select ss;
            bindingSource1.DataSource = sss;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns["computerComponent"].Visible = false;
            dataGridView1.Columns["description"].Visible = false;

            richTextBox1.DataBindings.Add("text", bindingSource1, "description");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            decimal price = 0;
            try
            {
                price = decimal.Parse(textBox1.Text);
                textBox1.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат в поле 'количества'.");
                return;
            }
            ShopStore shopStore = (ShopStore)dataGridView1.CurrentRow.DataBoundItem;
            DataService.db.ExecuteCommand("update computer_component set trade_price = {0} where id = {1}",
                price, shopStore.computerComponent.id);
            DataService.refreshAll();
            var sss = from ss in DataService.db.GetTable<ShopStore>()
                      select ss;
            bindingSource1.DataSource = sss;
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            PurchaseHistoryForm form = new PurchaseHistoryForm();
            form.Show();
        }

        private void создатьЗаявкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            CreateSupplyAssemblyOrderForm form = new CreateSupplyAssemblyOrderForm();
            form.Show();
        }

        private void заявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            SupplierApplicationForm form = new SupplierApplicationForm();
            form.Show();
        }
    }
}
