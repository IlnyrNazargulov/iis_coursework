using supplier_client.models;
using supplier_client.models.enums;
using supplier_client.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace supplier_client.forms
{
    public partial class SupplierStoreForm : Form
    {
        private ProducerService producerService;
        private ComputerComponentSevice computerComponentSevice;
        public SupplierStoreForm()
        {
            InitializeComponent();
            producerService = ProducerService.getProducerService();
            computerComponentSevice = ComputerComponentSevice.getComputerComponentSevice();
        }

        private void SupplierStoreForm_Load(object sender, EventArgs e)
        {
            List<Producer> producers = producerService.getProducers();
            var bs1 = new BindingSource();
            bs1.DataSource = producers;
            comboBox1.DataSource = bs1;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";

            List<String> types = new List<string>();
            foreach (ComponentType type in Enum.GetValues(typeof(ComponentType)))
            {
                types.Add(type.ToString());
            }
            var bs2 = new BindingSource();
            bs2.DataSource = types;
            comboBox2.DataSource = bs2;

            bindingSource1.DataSource = computerComponentSevice.getComputerComponentsInDataTable();
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[7].Visible = false;
            if (dataGridView1.Rows.Count > 0)
            {
                textBox4.DataBindings.Add("text", bindingSource1, "id", false);//id
                comboBox2.DataBindings.Add("text", bindingSource1, "Тип компонета"); //тип компонента
                textBox1.DataBindings.Add("text", bindingSource1, "Название компонента");//название  
                richTextBox1.DataBindings.Add("text", bindingSource1, "Описание");//описание
                comboBox1.DataBindings.Add("SelectedValue", bindingSource1, "ИД производитель"); //производитель
                textBox2.DataBindings.Add("text", bindingSource1, "Наша цена");//цена
                textBox3.DataBindings.Add("text", bindingSource1, "Количество на складе");//количество
                textBox5.DataBindings.Add("text", bindingSource1, "ИД компонента на складе", false);//id
            }
            if (!String.IsNullOrEmpty(textBox5.Text))
            {
                label8.Text = "На вашем складе имеется этот компонент";
            }
            else
            {
                label8.Text = "На вашем складе нет этого компонента";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            decimal supplierPrice;
            string name = textBox1.Text;
            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Название не должно быть пустым.");
                return;
            }
            try
            {
                supplierPrice = decimal.Parse(textBox2.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                MessageBox.Show("Цена должна иметь формат XX.XX.");
                textBox2.Text = "";
                return;
            }
            ComponentType componentType = (ComponentType)Enum.Parse(typeof(ComponentType), comboBox2.SelectedValue.ToString());
            int producerId = Convert.ToInt32(comboBox1.SelectedValue);
            string description = richTextBox1.Text;
            int count = Convert.ToInt32(textBox3.Text);
            if (count < 0)
            {
                MessageBox.Show("Количестов должно быть положительным числом.");
                return;
            }
            int? componentId = null;
            if (!String.IsNullOrEmpty(textBox4.Text))
            {
                componentId = Convert.ToInt32(textBox4.Text);
            }
            if (!String.IsNullOrEmpty(textBox5.Text))
            {
                componentId = null;
            }
            computerComponentSevice.createComputerComponentWithSupplier(componentId, supplierPrice, componentType, name, description, producerId, count);
            bindingSource1.DataSource = computerComponentSevice.getComputerComponentsInDataTable();
            dataGridView1.DataSource = bindingSource1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            decimal supplierPrice;
            string name = textBox1.Text;
            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Название не должно быть пустым.");
                return;
            }
            try
            {
                supplierPrice = decimal.Parse(textBox2.Text, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                MessageBox.Show("Цена должна иметь формат XX.XX.");
                textBox2.Text = "";
                return;
            }
            ComponentType componentType = (ComponentType)Enum.Parse(typeof(ComponentType), comboBox2.SelectedValue.ToString());
            int producerId = Convert.ToInt32(comboBox1.SelectedValue);
            string description = richTextBox1.Text;
            int count = Convert.ToInt32(textBox3.Text);
            if (count < 0)
            {
                MessageBox.Show("Количестов должно быть положительным числом.");
                return;
            }
            int componentId;
            try
            {
                componentId = Convert.ToInt32(textBox4.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Выберите компонент для обновления.");
                return;
            }

            int? supplierStoreId = null;
            if (!String.IsNullOrEmpty(textBox5.Text))
            {
                supplierStoreId = Convert.ToInt32(textBox5.Text);
            }
            computerComponentSevice.updateComputerComponent(componentId, supplierStoreId, supplierPrice, componentType, name, description, producerId, count);
            bindingSource1.DataSource = computerComponentSevice.getComputerComponentsInDataTable();
            dataGridView1.DataSource = bindingSource1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = computerComponentSevice.getComputerComponentsInDataTable();
            dataGridView1.DataSource = bindingSource1;
        }

        private void главнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("На вашем складе нет этого компонента.");
            }
            int supplierStoreId = Convert.ToInt32(textBox5.Text);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox5.Text))
            {
                label8.Text = "На вашем складе имеется этот компонент";
            }
            else
            {
                label8.Text = "На вашем складе нет этого компонента";
            }
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ClientOrderForm clientOrderForm = new ClientOrderForm();
            clientOrderForm.Show();
        }
    }
}
