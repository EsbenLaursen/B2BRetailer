using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Warehouse
{
    public class Warehouse
    {
        private string country;
        private int id;
        private IEnumerable<Product> products = null;
        private IBus bus = null;

        public Warehouse(int id, string country, IEnumerable<Product> products)
        {
            this.id = id;
            this.country = country;
            this.products = products;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus("host=localhost"))
            {
                // Listen for order request messages published to this warehouse only
                // using Topic Based Routing

                bus.Subscribe<OrderRequestMessageToLocalWarehouse>("warehouse" + id,
                                           HandleOrderEvent, x => x.WithTopic(country));

                // Listen for order request messages published by the retailer
                // to all warehouses. Notice that this subscriber subscribes to
                // another type of message (OrderBroadcastRequestMessage) than
                // the subscriber above (OrderRequestMessageToLocalWarehouse).
                // If they subscribed to the same message type, a pair of
                // subscribers with the same warehouse id would listen on the
                // same queue. 
                bus.Subscribe<OrderBroadcastRequestMessage>("warehouse" + id,
                    HandleOrderEvent);

                SynchronizedWriteLine("Warehouse " + id + " Listening for order requests");

                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderEvent(RetailerOrderRequestMessage message)
        {
            SynchronizedWriteLine("Order request received: (" +
                "Product Id: " + message.ProductId +
                " Country: " + message.CountryCode + ")"
                );

            int daysForDelivery = country == message.CountryCode ? 2 : 10;
            decimal shippingCharge = country == message.CountryCode ? 5 : 10;

            Product requestedProduct = products.FirstOrDefault(p => p.ProductId == message.ProductId);

            int itemsInStock = requestedProduct != null ? requestedProduct.ItemsInStock : 0;
            SynchronizedWriteLine("In stock: " + itemsInStock);
            OrderReplyMessage replyMessage = new OrderReplyMessage
            {
                WarehouseId = this.id,
                ProductId = message.ProductId,
                CustomerId = message.CustomerId,
                ItemsInStock = itemsInStock,
                DaysForDelivery = daysForDelivery,
                ShippingCharge = shippingCharge
            };
            
            bus.Send<OrderReplyMessage>("ReplyQueueWarehouse", replyMessage);
            SynchronizedWriteLine("Reply sent back to retailer");
        }
        
        private void SynchronizedWriteLine(string s)
        {
            lock (this)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }
    }
}
