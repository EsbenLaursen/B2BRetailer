using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Customer
{
    public class MessagingGateway
    {
        IBus bus = RabbitHutch.CreateBus("host=localhost");
        static OrderReplyMessage reply = null;
        public MessagingGateway(Customer c)
        {

            bus.Subscribe<OrderReplyMessage>("Customer" + c.Id, HandleRetailerResponse, x => x.WithTopic(c.Id + ""));


        }

        public OrderReplyMessage PlaceOrder(Customer c)
        {
            bool gotReply;
            var order = new CustomerOrderRequest() { CountryCode = c.Countrycode, CustomerId = c.Id, ProductId = c.ProductID };
            bus.Send("ReplyQueueCustomer", order);
            Console.WriteLine("Sent order to retailer, waiting for response...");

            lock (this)
            {
                gotReply = Monitor.Wait(this, 10000);
            }

            if (gotReply)
                return reply;
            else
                throw new Exception("Timeout. The requested product is out of stock!");

        }
        //move to customer
        private void HandleRetailerResponse(OrderReplyMessage replyMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("Answer from retailer: \n" +
                " Warehouse with id: {0} has {1} items in stock with the shipping charge of {2}"
                , replyMessage.WarehouseId, replyMessage.ItemsInStock, replyMessage.ShippingCharge
                ));
            reply = replyMessage;
            lock (this)
            {
                // Wake up the blocked thread which sent the order request message 
                // so that it can return the reply message to the application.
                Monitor.Pulse(this);
            }



        }
    }
}
