using customer_client.models;
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
    public partial class RegistrationForm : Form
    {

        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            goToLogin();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            string passwordRe = textBox3.Text;
            string fullName = textBox4.Text;
            if (String.IsNullOrEmpty(login))
            {
                MessageBox.Show("Логин не должен быть пустым.");
                return;
            }
            if (password != passwordRe)
            {
                MessageBox.Show("Пароль не совпадает.");
                return;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Минимальная длина пароля 6 символов.");
                return;
            }
          
            using (DataService.db.Transaction = DataService.db.Connection.BeginTransaction())
            {
                var query = from account in DataService.db.GetTable<Account>()
                            where account.login == login &&
                            account.isRemoved == false
                            select account;
                if (query.Count() != 0)
                {
                    MessageBox.Show("Аккаунт с таким логином уже существует.");
                    DataService.db.Transaction.Commit();
                    return;
                }
                Account newAccount = new Account()
                {
                    fullName = fullName,
                    login = login,
                    password = password,
                    createdAt = DateTime.Now,
                    type = AccountType.CLIENT
                };
                DataService.db.GetTable<Account>().InsertOnSubmit(newAccount);
                DataService.db.SubmitChanges();
                DataService.db.Transaction.Commit();
            }
            goToLogin();
        }
        private void goToLogin()
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
