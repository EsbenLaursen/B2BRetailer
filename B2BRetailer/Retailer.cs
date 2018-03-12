using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace B2BRetailer
{
    public class Retailer
    {
        private static List<int> outstandingOrders = new List<int>();
        static IBus bus = RabbitHutch.CreateBus("host=localhost");

        public void Start()
        {

            Console.WriteLine("Welcome to retailer");
            Console.WriteLine("Listening for customer request...");

            bus.Receive<CustomerOrderRequest>("ReplyQueueCustomer", m => HandleReplyCustomer(m));
            bus.Receive<OrderReplyMessage>("ReplyQueueWarehouse", m => HandleWarehouseReply(m));
            
            lock (this)
            {
                Monitor.Wait(this);
            }
        }

        static void HandleReplyCustomer(CustomerOrderRequest requestMessage)
        {
            Console.WriteLine(string.Format("Got customer order request: Customer id: {0} Country code: {1} Product id {2}\n" +
                "trying to send to local warehouse", requestMessage.CustomerId, requestMessage.CountryCode, requestMessage.ProductId));

            OrderRequestMessageToLocalWarehouse ormtlw = new OrderRequestMessageToLocalWarehouse();
            ormtlw.CountryCode = requestMessage.CountryCode;
            ormtlw.CustomerId = requestMessage.CustomerId;
            ormtlw.ProductId = requestMessage.ProductId;
            ormtlw.ReplyTo = requestMessage.CustomerId + "";


            bus.Publish<OrderRequestMessageToLocalWarehouse>(ormtlw, ormtlw.CountryCode);
            outstandingOrders.Add(requestMessage.CustomerId);
            Console.WriteLine("Send to local warehouse: " + requestMessage.CountryCode);


        }
        private static void HandleWarehouseReply(OrderReplyMessage replyMessage)
        {
            if (replyMessage.ItemsInStock > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Answer from warehouse: \n" +
                    "Warehouse id: {0} " +
                    "Items in stock: {1} " +
                    "Days for delivery: {2} " +
                    "Shipping charge: {3}", replyMessage.WarehouseId, replyMessage.ItemsInStock,
                    replyMessage.DaysForDelivery, replyMessage.ShippingCharge));
                if (outstandingOrders.Contains(replyMessage.CustomerId))
                {
                    bus.Publish<OrderReplyMessage>(replyMessage, replyMessage.CustomerId + "");
                    outstandingOrders.Remove(replyMessage.CustomerId);
                }
            }
            else if (replyMessage.DaysForDelivery == 2)
            {
                if (replyMessage.ItemsInStock == 0)
                {
                    Console.WriteLine("No product in stock in local warehouse - Publishing to all warehouses");

                    OrderBroadcastRequestMessage e = new OrderBroadcastRequestMessage();
                    e.CustomerId = replyMessage.CustomerId;
                    e.OrderId = replyMessage.OrderId;
                    e.ProductId = replyMessage.ProductId;
                    bus.Publish<OrderBroadcastRequestMessage>(e);
                }
            }
        }

    }
}
