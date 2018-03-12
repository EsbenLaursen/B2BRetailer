using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class SynchronousMessagingGateway
    {
        IBus bus = RabbitHutch.CreateBus("host=localhost");
        CustomerOrderRequest replyMessage = null;
        int timeout = 5000;

        public SynchronousMessagingGateway()
        {
            bus.Receive<CustomerOrderRequest>("ReplyQueue", m => HandleReply(m));
        }

        public string SendRequest(string message, string topic)
        {
            CustomerOrderRequest requestMessage = new CustomerOrderRequest { Name = message, CountryCode = topic }; //CustomerId
            bool gotReply;

            bus.Publish<CustomerOrderRequest>(requestMessage, topic);

            lock (this)
            {
                gotReply = Monitor.Wait(this, timeout);
            }

            if (gotReply)
                return replyMessage.ProductId;
            else
                throw new Exception("Timeout!");
        }

        public void Close()
        {
            if (bus != null)
                bus.Dispose();
        }

        private void HandleReply(CustomerOrderRequest reply)
        {
            replyMessage = reply;



            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
    }
}
