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

namespace supplier_client.forms
{
    public partial class MainForm : Form
    {
        private AccountService accountServise;
        private SupplierService supplierService;

        public MainForm()
        {
            InitializeComponent();
            accountServise = AccountService.getAccountService();
            supplierService = SupplierService.getSupplierService();
        }

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            SupplierStoreForm supplierStoreForm = new SupplierStoreForm();
            supplierStoreForm.Show();
        }

        private void заявкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ClientOrderForm clientOrderForm = new ClientOrderForm();
            clientOrderForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Account account = accountServise.getAccountById(AccountService.accountId);
            Supplier supplier = supplierService.getSupplierByAccountId(AccountService.accountId);
            textBox1.Text = account.fullName;
            textBox2.Text = account.login;
            textBox3.Text = account.password;
            textBox4.Text = supplier.name;
            richTextBox1.Text = supplier.description;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("ФИО не должен быть пустым.");
                return;
            }
            if (String.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Название поставщика не должно быть пустым.");
                return;
            }
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Логин не должен быть пустым.");
                return;
            }
            if (String.IsNullOrEmpty(textBox3.Text) || textBox3.Text.Length < 6)
            {
                MessageBox.Show("Пароль не должен быть пустым и минимальная длина пароля 6 символов.");
                return;
            }
            string fullName = textBox1.Text;
            string login = textBox2.Text;
            string password = textBox3.Text;
            string supplierName = textBox4.Text;
            string supplierDescription = richTextBox1.Text;
            accountServise.updateAccountAndSupplier(login, password, fullName, supplierName, supplierDescription);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "После удаления произойдет переход на форму логина. В этот аккаунт больше нельзя будет зайти! Вы действительно хотите удалить аккаунт?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
                );
            if (result == DialogResult.Yes)
            {
                accountServise.deleteAccountAndSupplier();
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
