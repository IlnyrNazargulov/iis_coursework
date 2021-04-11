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
    public partial class LoginForm : Form
    {

        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            var query = from account in DataService.db.GetTable<Account>()
                        where account.login == login &&
                        account.password == password &&
                        account.isRemoved == false &&
                        account.type == AccountType.SELLER
                        select account;
            if (query.Count() != 0)
            {
                DataService.currentAccount = query.Single();
                goToMain();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            goToRegistration();
        }

        private void goToMain()
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void goToRegistration()
        {
            this.Hide();
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
        }
    }
}
