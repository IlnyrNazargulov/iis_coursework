using supplier_client.exceptions;
using supplier_client.models;
using supplier_client.models.enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.services
{
    class AccountService
    {
        public static int accountId;

        private static AccountService accountService;

        private static string GET_ACCOUNT_BY_LOGIN_AND_PASSWORD = "SELECT * FROM account " +
            "WHERE login = @login AND password = @password and is_removed = 0 and type = 'SUPPLIER'";
        private static string GET_ACCOUNT_BY_LOGIN = "select * from account where login = @login and is_removed = 0";
        private static string GET_ACCOUNT_BY_ID = "select * from account where id = @id";


        private static string CREATE_ACCOUNT = "INSERT INTO account(type, created_at, login, password, full_name) " +
            "VALUES(@type, @created_at, @login, @password, @full_name);SELECT SCOPE_IDENTITY()";
        private static string UPDATE_ACCOUNT = "UPDATE account SET login = @login, full_name = @full_name, password = @password " +
            "WHERE id = @id";
        private static string UPDATE_ACCOUNT_IS_REMOVED = "UPDATE account SET is_removed = 1 " +
            "WHERE id = @id";

        private SupplierService supplierService;

        public AccountService()
        {
            supplierService = SupplierService.getSupplierService();
        }

        public static AccountService getAccountService()
        {
            if (accountService == null)
            {
                accountService = new AccountService();
                return accountService;
            }
            return accountService;
        }

        public void updateAccountAndSupplier(
            String login,
            String password,
            String fullName,
            String nameSupplier,
            String descriptionSupplier
            )
        {
            DataService.createTransaction();
            SqlCommand command = DataService.createSqlCommand(UPDATE_ACCOUNT);
            DataService.addParameter("@login", login);
            DataService.addParameter("@password", password);
            DataService.addParameter("@full_name", fullName);
            DataService.addParameter("@id", AccountService.accountId);
            command.ExecuteNonQuery();
            supplierService.updateSupplier(nameSupplier, descriptionSupplier);
            DataService.commitTransaction();
        }

        public void deleteAccountAndSupplier()
        {
            DataService.createTransaction();       
            SqlCommand command = DataService.createSqlCommand(UPDATE_ACCOUNT_IS_REMOVED);
            DataService.addParameter("@id", AccountService.accountId);
            command.ExecuteNonQuery();
            supplierService.deleteSupplier();
            DataService.commitTransaction();
        }

        public void login(String login, String password)
        {
            SqlCommand command = DataService.createSqlCommand(GET_ACCOUNT_BY_LOGIN_AND_PASSWORD);
            DataService.addParameter("@login", login);
            DataService.addParameter("@password", password);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    accountId = Convert.ToInt32(reader["id"]);
                }
                reader.Close();
                Supplier supplier = supplierService.getSupplierByAccountId(accountId);
                SupplierService.supplierId = supplier.id;
                return;
            }
            else
            {
                reader.Close();
                throw new LoginException();
            }
        }

        public void registration(String login, String password, String fullName, String supplierName, String supplierDescription)
        {
            DataService.createTransaction();
            Account account = getAccountByLogin(login);
            Supplier supplier = supplierService.getSupplierByName(supplierName);
            if (supplier != null)
            {
                DataService.commitTransaction();
                throw new SupplierAlreadyExist();
            }
            if (account != null)
            {
                DataService.commitTransaction();
                throw new LoginAlreadyExistException();
            }
            int accountId = createAccount(login, password, fullName, AccountType.SUPPLIER);
            supplierService.createSupplier(supplierName, supplierDescription, accountId);

            DataService.commitTransaction();
        }
        private int createAccount(String login, String password, String fullName, AccountType type)
        {
            SqlCommand command = DataService.createSqlCommand(CREATE_ACCOUNT);
            DataService.addParameter("@login", login);
            DataService.addParameter("@password", password);
            DataService.addParameter("@type", type.ToString());
            DataService.addParameter("@created_at", DateTime.Now);
            DataService.addParameter("@full_name", fullName);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private Account getAccountByLogin(String loginParam)
        {
            SqlCommand command = DataService.createSqlCommand(GET_ACCOUNT_BY_LOGIN);
            DataService.addParameter("@login", loginParam);
            SqlDataReader reader = command.ExecuteReader();
            Account account = extractAccount(reader);
            return account;
        }

        public Account getAccountById(int idParam)
        {
            SqlCommand command = DataService.createSqlCommand(GET_ACCOUNT_BY_ID);
            DataService.addParameter("@id", idParam);
            SqlDataReader reader = command.ExecuteReader();
            Account account = extractAccount(reader);
            return account;
        }

        private Account extractAccount(SqlDataReader reader)
        {
            Account account = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["id"]);
                    AccountType type = (AccountType)Enum.Parse(typeof(AccountType), reader["type"].ToString());
                    DateTime createdAt = Convert.ToDateTime(reader["created_at"]);
                    string login = reader["login"].ToString();
                    string password = reader["password"].ToString();
                    string fullName = reader["full_name"].ToString();
                    account = new Account(id, type, createdAt, login, password, fullName);
                }
            }
            else
            {
                reader.Close();
                return null;
            }
            reader.Close();
            return account;
        }
    }
}
