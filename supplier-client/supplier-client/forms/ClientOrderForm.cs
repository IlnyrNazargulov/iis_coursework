using supplier_client.models.enums;
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

namespace supplier_client.forms
{
    public partial class ClientOrderForm : Form
    {
        private SupplyAssemblyOrderService supplyAssemblyOrderService;
        private ComputerComponentSevice computerComponentSevice;

        public ClientOrderForm()
        {
            InitializeComponent();
            supplyAssemblyOrderService = SupplyAssemblyOrderService.getSupplyAssemblyOrderService();
            computerComponentSevice = ComputerComponentSevice.getComputerComponentSevice();
        }

        private void ClientOrderForm_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = supplyAssemblyOrderService.getSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
                int orderId = Convert.ToInt32(dataGridView1.Rows[0].Cells["id"].Value);
                DataTable dataTable = computerComponentSevice.getComputerComponentsByOrderId(orderId);
                dataGridView2.DataSource = dataTable;
            }
            updateRow();
        }

        private void updateRow()
        {
            if (dataGridView1.CurrentRow == null)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button5.Enabled = false;
                return;
            }
            SupplyAssemblyOrderStatus status = (SupplyAssemblyOrderStatus)Enum.Parse(typeof(SupplyAssemblyOrderStatus), dataGridView1.CurrentRow.Cells["Статус"].Value.ToString());
            if (status != SupplyAssemblyOrderStatus.CREATED)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
            if (status != SupplyAssemblyOrderStatus.ACCEPT)
            {
                button5.Enabled = false;
            }
            else
            {
                button5.Enabled = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = supplyAssemblyOrderService.getAvailableSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
            updateRow();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = supplyAssemblyOrderService.getInaccessibleSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
            updateRow();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = supplyAssemblyOrderService.getSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
            updateRow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int supplyAssemblyOrderId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            supplyAssemblyOrderService.updateStatusSupplyAssemblyOrders(supplyAssemblyOrderId, SupplyAssemblyOrderStatus.ACCEPT);
            formToDefault();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int supplyAssemblyOrderId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            supplyAssemblyOrderService.updateStatusSupplyAssemblyOrders(supplyAssemblyOrderId, SupplyAssemblyOrderStatus.DECLINE);
            formToDefault();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int supplyAssemblyOrderId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            int check = supplyAssemblyOrderService.checkOrder(supplyAssemblyOrderId);
            if (check < 0)
            {
                MessageBox.Show("Недостаточно компонентов на складе.");
                return;
            }
            DateTime deviveredAt = dateTimePicker1.Value;
            supplyAssemblyOrderService.updateStatusAndDeliveredAtSupplyAssemblyOrders(supplyAssemblyOrderId, SupplyAssemblyOrderStatus.SENT, deviveredAt);
            formToDefault();
            supplyAssemblyOrderService.updateSupplierStore(supplyAssemblyOrderId);
        }

        private void formToDefault()
        {
            radioButton3.Checked = true;
            dataGridView1.DataSource = supplyAssemblyOrderService.getSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
            updateRow();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = supplyAssemblyOrderService.getSendedSupplyAssemblyOrdersInDataTable();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
            updateRow();
        }

        private void главнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            SupplierStoreForm supplierStoreForm = new SupplierStoreForm();
            supplierStoreForm.Show();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            updateRow();
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            int orderId = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["id"].Value);
            DataTable dataTable = computerComponentSevice.getComputerComponentsByOrderId(orderId);
            dataGridView2.DataSource = dataTable;
        }
    }
}
