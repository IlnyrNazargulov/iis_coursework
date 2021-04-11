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
    public partial class RegistrationForm : Form
    {
        private AccountService accountService;

        public RegistrationForm()
        {
            InitializeComponent();
            accountService = AccountService.getAccountService();
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
            string nameSupplier = textBox5.Text;
            string descriptionSupplier = richTextBox1.Text;
            if (String.IsNullOrEmpty(login))
            {
                MessageBox.Show("Логин не должен быть пустым.");
                return;
            }
            if (String.IsNullOrEmpty(nameSupplier))
            {
                MessageBox.Show("Название поставщика не должно быть пустым.");
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
            try
            {
                accountService.registration(login, password, fullName, nameSupplier, descriptionSupplier);
                goToLogin();
            }
            catch (LoginAlreadyExistException)
            {
                MessageBox.Show("Аккаунт с таким логином уже существует.");
            }
            catch (SupplierAlreadyExist)
            {
                MessageBox.Show("Поставщик с таким именем уже существует.");
            }
        }
        private void goToLogin()
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
