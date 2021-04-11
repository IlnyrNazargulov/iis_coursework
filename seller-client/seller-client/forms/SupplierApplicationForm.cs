using seller_client.models;
using seller_client.models.enums;
using seller_client.services;
using System;
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
    public partial class SupplierApplicationForm : Form
    {
        public SupplierApplicationForm()
        {
            InitializeComponent();
        }

        private void SupplierApplicationForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            updateForm();
        }
        private void updateControl()
        {
            if (dataGridView1.CurrentRow == null)
            {
                return;
            }
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            if (order.status == SupplyAssemblyOrderStatus.SENT)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
            if (order.status == SupplyAssemblyOrderStatus.NEW)
            {
                button1.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
        }

        private void updateForm()
        {
            var sads = from sad in DataService.db.GetTable<SupplyAssemblyOrder>()
                       select sad;
            DataService.db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, sads);

            dataGridView1.DataSource = sads;

            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[0].Selected = true;
                updateDataGrid2();
            }
            updateControl();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            updateControl();
            updateDataGrid2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SupplierStoreComponentInOrder componentInOrder = (SupplierStoreComponentInOrder)dataGridView2.CurrentRow.DataBoundItem;
            DataService.db.ExecuteCommand("delete from supply_assembly_details where id = {0}", componentInOrder.supplyAssemblyDetailsId);
            updateDataGrid2();
        }
        private void updateDataGrid2()
        {
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            var sscio = from sad in DataService.db.GetTable<SupplyAssemblyDetails>()
                        where sad.supplyAssemblyOrderId == order.id
                        join st in DataService.db.GetTable<SupplierStore>() on sad.supplierStoreId equals st.id
                        select new SupplierStoreComponentInOrder(st.computerComponent.id, st.computerComponent.type, st.computerComponent.name, st.supplierPrice, sad.id, sad.count);
            dataGridView2.DataSource = sscio;
            if (dataGridView2.Rows.Count != 0)
            {
                dataGridView2.Rows[0].Selected = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            DataService.db.ExecuteCommand("delete from supply_assembly_details where supply_assembly_order_id = {0}", order.id);
            DataService.db.SubmitChanges();
            updateDataGrid2();
        }

        private void главнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            order.status = SupplyAssemblyOrderStatus.CANCEL;
            DataService.db.SubmitChanges();
            updateForm();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Заявка пуста.");
                return;
            }

            order.status = SupplyAssemblyOrderStatus.CREATED;
            DataService.db.Refresh(System.Data.Linq.RefreshMode.KeepChanges, order);
            DataService.db.SubmitChanges();
            updateForm();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var sads = from sad in DataService.db.GetTable<SupplyAssemblyOrder>()
                       where sad.status == SupplyAssemblyOrderStatus.NEW
                       select sad;
            dataGridView1.DataSource = sads;
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[0].Selected = true;
                updateDataGrid2();
            }
            updateControl();
        }
        //все
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            updateForm();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            var sads = from sad in DataService.db.GetTable<SupplyAssemblyOrder>()
                       where sad.status != SupplyAssemblyOrderStatus.CANCEL &&
                       sad.status != SupplyAssemblyOrderStatus.NEW &&
                       sad.status != SupplyAssemblyOrderStatus.ARRIVED
                       select sad;
            dataGridView1.DataSource = sads;
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[0].Selected = true;
                updateDataGrid2();
            }
            updateControl();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SupplyAssemblyOrder order = (SupplyAssemblyOrder)dataGridView1.CurrentRow.DataBoundItem;
            var sscios = from sad in DataService.db.GetTable<SupplyAssemblyDetails>()
                         where sad.supplyAssemblyOrderId == order.id
                         join st in DataService.db.GetTable<SupplierStore>() on sad.supplierStoreId equals st.id
                         select new SupplierStoreComponentInOrder(st.computerComponent.id, st.computerComponent.type, st.computerComponent.name, st.supplierPrice, sad.id, sad.count);
            foreach (var sscio in sscios)
            {
                var ss = DataService.db.GetTable<ShopStore>()
                    .Where(ssP => ssP.computerComponentId == sscio.id)
                    .SingleOrDefault();
                if (ss == null)
                {
                    var shopStore = new ShopStore(sscio.id, sscio.count);
                    DataService.db.GetTable<ShopStore>().InsertOnSubmit(shopStore);
                }
                else
                {
                    ss.count = ss.count + sscio.count;
                }
            }
            order.status = SupplyAssemblyOrderStatus.ARRIVED;
            DataService.db.SubmitChanges();
            updateForm();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            var sads = from sad in DataService.db.GetTable<SupplyAssemblyOrder>()
                       where sad.status == SupplyAssemblyOrderStatus.ARRIVED
                       select sad;
            dataGridView1.DataSource = sads;
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[0].Selected = true;
                updateDataGrid2();
            }
            updateControl();
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
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            StoreForm form = new StoreForm();
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
