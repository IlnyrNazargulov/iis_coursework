using supplier_client.exceptions;
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
    public partial class LoginForm : Form
    {
        private AccountService accountService;

        public LoginForm()
        {
            InitializeComponent();
            accountService = AccountService.getAccountService();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            try
            {
                accountService.login(login, password);
                goToMain();
            }
            catch (LoginException)
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
