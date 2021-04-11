using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seller_client.models
{
    [Table(Name = "supplier")]
    class Supplier
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }
        [Column(Name = "name")]
        public string name { get; set; }
        [Column(Name = "is_removed")]
        public bool isRemoved { get; set; }
        [Column(Name = "description")]
        public string description { get; set; }
    }
}