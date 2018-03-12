using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class CustomerOrderRequest
    {
        public int ProductId { get; set; }
        public string CountryCode { get; set; }
        public int CustomerId { get; set; }
        public bool IssentToAll { get; set; }
    }

    public class RetailerOrderRequestMessage : CustomerOrderRequest
    {
        public int OrderId { get; set; }
        public string ReplyTo { get; set; }
    }



    // Order request message from retailer to local warehouse
    public class OrderRequestMessageToLocalWarehouse : RetailerOrderRequestMessage
    {
    }

    // Order request message from retailer to all warehouses
    public class OrderBroadcastRequestMessage : RetailerOrderRequestMessage
    {
    }
}
