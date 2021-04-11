using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace supplier_client.models.enums
{
    enum SupplyAssemblyOrderStatus
    {
        NEW,
        CREATED,
        CANCEL,
        DECLINE,
        ACCEPT,
        SENT,
        ARRIVED
    }
}
