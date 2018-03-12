using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public class Customer
    {
        public int Id;
        public int ProductID;
        public string Countrycode;

        public static MessagingGateway gateway;

        public Customer(int Id, int ProductID, string Countrycode)
        {
            this.Id = Id;
            this.ProductID = ProductID;
            this.Countrycode = Countrycode;
        }

        public void Start()
        {
            gateway = new MessagingGateway(this);
            WriteToConsole("Customer " + Id + " running");
            gateway.PlaceOrder(this);
        }

        private void WriteToConsole(string s)
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
