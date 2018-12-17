using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Factory
{
    public class HumanFactory : IFactory
    {
        public virtual IRace CreateRace()
        {
            return new Human();
        }
    }
    public class HumanFactoryAdvanced: HumanFactory
    {
        public override IRace CreateRace()
        {
            Console.WriteLine("123");
            return new Human();
        }
    }
}
