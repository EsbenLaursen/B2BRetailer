using EasyNetQ;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse
{
    class WarehouseProgram
    {
        static IBus bus = null;
        private static int Id { get; set; }
        public static string CountryCode { get; set; }

        static void Main(string[] args)
        {

            List<Product> dkProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 }
            };

            List<Product> frProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 },
                new Product { ProductId = 2, ItemsInStock = 2 }
            };
            List<Product> usProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 },
                new Product { ProductId = 2, ItemsInStock = 2 },
                new Product { ProductId = 3, ItemsInStock = 5 }
            };

            Task.Factory.StartNew(() => new Warehouse(1, "DK", dkProducts).Start());
            Task.Factory.StartNew(() => new Warehouse(2, "FR", frProducts).Start());
            Task.Factory.StartNew(() => new Warehouse(3, "US", usProducts).Start());

            Console.ReadLine(); 
        }
       
      
        
    }
}
