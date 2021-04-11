using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models
{
    [Table(Name = "producer")]
    class Producer
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "id")]
        public int id { get; set; }
        [Column(Name = "name")]
        public string name { get; set; }
    }
}
