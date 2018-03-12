using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class OrderReplyMessage
    {
        public int WarehouseId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int ItemsInStock { get; set; }
        public int DaysForDelivery { get; set; }
        public decimal ShippingCharge { get; set; }
    }
}
