using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProtoType.Product;

namespace ProtoType
{
    public class Client
    {
        public static void Main(string[] args)
        {
            Product p1 = new Product()
            {
                Name = "1",
                Age = 1,
                Number = new NumberFlag() { Num = "01" }
            };

            var p2 = p1.Clone() as Product;

            if (p2 != null)
            {

                p2.Number.Num = "22";

                Console.WriteLine("p1 Number:{0}.", p1.Number.Num);

                Console.WriteLine("p2 Number:{0}.", p2.Number.Num);

                Console.ReadKey();
            }

        }
    }
}
