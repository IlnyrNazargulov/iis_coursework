using customer_client.models.enums;
using supplier_client.forms;
using supplier_client.models;
using supplier_client.models.enums;
using supplier_client.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace customer_client.forms
{
    public partial class ShopForm : Form
    {
        public ShopForm()
        {
            InitializeComponent();
        }

        private void ShopForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            var ccs = from cc in DataService.db.GetTable<ComputerComponent>()
                      where cc.tradePrice != null
                      select cc;

            bindingSource1.DataSource = ccs;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns["description"].Visible = false;

            richTextBox1.DataBindings.Add("text", bindingSource1, "description");//описание

            List<String> types = new List<string>();
            foreach (ComponentType type in Enum.GetValues(typeof(ComponentType)))
            {
                types.Add(type.ToString());
            }
            var bs1 = new BindingSource();
            bs1.DataSource = types;
            comboBox1.DataSource = bs1;

            var producers = DataService.db.GetTable<Producer>();
            var bs2 = new BindingSource();
            bs2.DataSource = producers;
            comboBox2.DataSource = bs2;
            comboBox2.DisplayMember = "name";
            comboBox2.ValueMember = "id";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ComponentType componentType = (ComponentType)Enum.Parse(typeof(ComponentType), comboBox1.SelectedValue.ToString());
            int producerId = Convert.ToInt32(comboBox2.SelectedValue);
            Decimal? lowBoundary = null;
            Decimal? upperBoundary = null;
            try
            {
                if (!String.IsNullOrEmpty(textBox2.Text))
                {
                    lowBoundary = decimal.Parse(textBox2.Text, CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Цена должна иметь формат XX.XX.");
                textBox2.Text = "";
                return;
            }
            try
            {
                if (!String.IsNullOrEmpty(textBox3.Text))
                {
                    upperBoundary = decimal.Parse(textBox3.Text, CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Цена должна иметь формат XX.XX.");
                textBox2.Text = "";
                return;
            }
            var ccs = from cc in DataService.db.GetTable<ComputerComponent>()
                      where (lowBoundary == null || cc.tradePrice > lowBoundary) &&
                      (upperBoundary == null || cc.tradePrice < upperBoundary) &&
                      cc.type == componentType &&
                      cc.producer.id == producerId &&
                      cc.tradePrice != null
                      select cc;
            bindingSource1.DataSource = ccs;
            richTextBox1.DataBindings.Clear();
            richTextBox1.DataBindings.Add("text", bindingSource1, "description");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ccs = from cc in DataService.db.GetTable<ComputerComponent>()
                      where cc.tradePrice != null
                      select cc;
            bindingSource1.DataSource = ccs;
            richTextBox1.DataBindings.Clear();
            richTextBox1.DataBindings.Add("text", bindingSource1, "description");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            int componentId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            var components = from cc in DataService.db.GetTable<ComputerComponent>()
                             where cc.id == componentId
                             select cc;
            try
            {
                count = int.Parse(textBox1.Text);
                textBox1.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат в поле 'количества'.");
            }
            ClientOrder order = DataService.order;
            var existCad = DataService.db.GetTable<ClientAssemblyDetails>()
               .Where(cadP => cadP.computerComponentId == componentId && cadP.clientOrderId == order.id).SingleOrDefault();
            if (existCad != null)
            {
                existCad.count = existCad.count + count;
            }
            else
            {
                var cad = new ClientAssemblyDetails(count, components.Single().id, order.id);
                DataService.db.GetTable<ClientAssemblyDetails>().InsertOnSubmit(cad);
            }
            DataService.db.SubmitChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientOrder order = DataService.order;

            int componentId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            var cad = DataService.db.GetTable<ClientAssemblyDetails>()
                .Where(cadP => cadP.computerComponentId == componentId && cadP.clientOrderId == order.id).SingleOrDefault();
            if (cad == null)
            {
                MessageBox.Show("В корзине нет такого компонента.");
                return;
            }
            DataService.db.ExecuteCommand("delete from client_assembly_details where client_order_id = {0} and " +
                "computer_component_id = {1}", cad.clientOrderId, cad.computerComponentId);

            DataService.db.SubmitChanges();
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
            BasketForm basketForm = new BasketForm();
            basketForm.Show();
        }

        private void историяПокупокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            PurchaseHistoryForm purchaseHistoryForm = new PurchaseHistoryForm();
            purchaseHistoryForm.Show();
        }
    }
}
