using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer
{
    public class CustomerProgram
    {
       
     
        static void Main(string[] args)
        {
            Console.WriteLine("Customer console");
            Task.Factory.StartNew(() => new Customer(1, 2, "DK").Start());
            //Task.Factory.StartNew(() => new Customer(2, 1, "DK").Start());
            //Task.Factory.StartNew(() => new Customer(3, 100, "DK").Start());

            Console.ReadLine();
        }
       
    }
}
