using seller_client.models;
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
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("ФИО не должен быть пустым.");
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

            using (DataService.db.Transaction = DataService.db.Connection.BeginTransaction())
            {
                var query = from account in DataService.db.GetTable<Account>()
                            where account.login == login &&
                            account.isRemoved == false
                            select account;
                if (query.Count() != 0 && query.Single().id != DataService.currentAccount.id)
                {
                    MessageBox.Show("Аккаунт с таким логином уже существует.");
                    DataService.db.Transaction.Commit();
                    return;
                }
                var acc = DataService.db.GetTable<Account>()
                    .Where(a => a.login == DataService.currentAccount.login)
                    .SingleOrDefault();
                acc.login = login;
                acc.fullName = fullName;
                acc.password = password;
                DataService.currentAccount = acc;
                DataService.db.SubmitChanges();
                DataService.db.Transaction.Commit();
            }
            MessageBox.Show("Информация успешно обновлена.");
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
                DataService.currentAccount.isRemoved = true;
                DataService.db.SubmitChanges();
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataService.refreshAll();
            textBox1.Text = DataService.currentAccount.fullName;
            textBox2.Text = DataService.currentAccount.login;
            textBox3.Text = DataService.currentAccount.password;
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

        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            StoreForm form = new StoreForm();
            form.Show();
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            PurchaseHistoryForm form = new PurchaseHistoryForm();
            form.Show();
        }
    }
}
