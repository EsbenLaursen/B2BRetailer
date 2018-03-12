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
    class RetailerProgram
    {
       
        static void Main(string[] args)
        {
            Retailer retailer = new Retailer();
            retailer.Start();

          

        }
        
        
    }
}
