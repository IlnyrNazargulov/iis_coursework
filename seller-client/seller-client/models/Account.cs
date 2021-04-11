using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seller_client.models
{
    [Table(Name = "account")]
    class Account
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id { get; set; }
        [Column(Name = "is_removed")]
        public bool isRemoved { get; set; }
        [Column(Name = "type", DbType = "varchar(63)")]
        public AccountType type { get; set; }
        [Column(Name = "created_at")]
        public DateTime createdAt { get; set; }
        [Column(Name = "login")]
        public string login { get; set; }
        [Column(Name = "password")]
        public string password { get; set; }
        [Column(Name = "full_name")]
        public string fullName { get; set; }
    }
}
