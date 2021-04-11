using supplier_client.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models
{
    class Account
    {
        public int id { get; set; }
        public AccountType type { get; set; }
        public DateTime createdAt { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }

        public Account(int id, AccountType type, DateTime createdAt, string login, string password, string fullName)
        {
            this.id = id;
            this.type = type;
            this.createdAt = createdAt;
            this.login = login;
            this.password = password;
            this.fullName = fullName;
        }
    }
}
