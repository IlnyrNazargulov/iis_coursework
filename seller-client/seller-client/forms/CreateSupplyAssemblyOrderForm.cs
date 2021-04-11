using seller_client.models;
using seller_client.models.enums;
using seller_client.services;
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

namespace seller_client.forms
{
    public partial class CreateSupplyAssemblyOrderForm : Form
    {
        public CreateSupplyAssemblyOrderForm()
        {
            InitializeComponent();
        }

        private void ShopForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            var ccws = from cc in DataService.db.GetTable<ComputerComponent>()
                       join st in DataService.db.GetTable<SupplierStore>() on cc.id equals st.computerComponentId
                       select new ComputerComponentWithSupplier(cc.id, cc.type, cc.name, cc.description, st.supplierPrice, cc.producer.name, st.supplier, st.id);

            bindingSource1.DataSource = ccws;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns["description"].Visible = false;

            richTextBox1.DataBindings.Add("text", bindingSource1, "description");

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

            var suppliers = DataService.db.GetTable<Supplier>();
            comboBox3.DataSource = suppliers;
            comboBox3.DisplayMember = "name";
            comboBox3.ValueMember = "id";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ComponentType componentType = (ComponentType)Enum.Parse(typeof(ComponentType), comboBox1.SelectedValue.ToString());
            int producerId = Convert.ToInt32(comboBox2.SelectedValue);
            int supplierId = Convert.ToInt32(comboBox3.SelectedValue);

            var ccws = from cc in DataService.db.GetTable<ComputerComponent>()
                       where cc.producerId == producerId && cc.type == componentType
                       join st in DataService.db.GetTable<SupplierStore>() on cc.id equals st.computerComponentId
                       where st.supplierId == supplierId
                       select new ComputerComponentWithSupplier(cc.id, cc.type, cc.name, cc.description, st.supplierPrice, cc.producer.name, st.supplier, st.id);

            bindingSource1.DataSource = ccws;
            richTextBox1.DataBindings.Clear();
            richTextBox1.DataBindings.Add("text", bindingSource1, "description");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ccws = from cc in DataService.db.GetTable<ComputerComponent>()
                       join st in DataService.db.GetTable<SupplierStore>() on cc.id equals st.computerComponentId
                       select new ComputerComponentWithSupplier(cc.id, cc.type, cc.name, cc.description, st.supplierPrice, cc.producer.name, st.supplier, st.id);

            bindingSource1.DataSource = ccws;

            richTextBox1.DataBindings.Clear();
            richTextBox1.DataBindings.Add("text", bindingSource1, "description");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            try
            {
                count = int.Parse(textBox1.Text);
                textBox1.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат в поле 'количества'.");
            }

            int componentId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            ComputerComponentWithSupplier computerComponentWithSupplier = (ComputerComponentWithSupplier)dataGridView1.CurrentRow.DataBoundItem;

            var supplierOrder = DataService.db.GetTable<SupplyAssemblyOrder>()
               .Where(so => so.supplierId == computerComponentWithSupplier.supplier.id && so.status == SupplyAssemblyOrderStatus.NEW).SingleOrDefault();
            if (supplierOrder == null)
            {
                DateTime now = DateTime.Now;
                string name = "Заказ_от_" + now.ToShortDateString();
                supplierOrder = new SupplyAssemblyOrder(name, now, SupplyAssemblyOrderStatus.NEW, computerComponentWithSupplier.supplier);
                DataService.db.GetTable<SupplyAssemblyOrder>().InsertOnSubmit(supplierOrder);
                DataService.db.SubmitChanges();
            }

            var supplyAssemblyDetails = DataService.db.GetTable<SupplyAssemblyDetails>()
              .Where(sad => sad.supplierStoreId == computerComponentWithSupplier.supplierStoreId && sad.supplyAssemblyOrderId == supplierOrder.id).SingleOrDefault();


            if (supplyAssemblyDetails != null)
            {
                supplyAssemblyDetails.count = supplyAssemblyDetails.count + count;
            }
            else
            {
                var sad = new SupplyAssemblyDetails(count, computerComponentWithSupplier.supplierStoreId, supplierOrder.id);
                DataService.db.GetTable<SupplyAssemblyDetails>().InsertOnSubmit(sad);
            }
            DataService.db.SubmitChanges();
        }

        private void создатьЗаявкуToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void заявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            SupplierApplicationForm form = new SupplierApplicationForm();
            form.Show();
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            StoreForm form = new StoreForm();
            form.Show();
        }
    }
}
