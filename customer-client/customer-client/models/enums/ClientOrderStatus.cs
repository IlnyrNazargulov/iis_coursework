using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_client.models.enums
{
    enum ClientOrderStatus
    {
        NEW,
        CREATED,
        CANCEL,
        DECLINE,
        ACCEPT,
        PAID,
        DONE,
        RECEIVED
    }
}
