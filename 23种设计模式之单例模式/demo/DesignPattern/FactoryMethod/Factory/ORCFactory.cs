using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Factory
{
    public class ORCFactory : IFactory
    {
        public IRace CreateRace()
        {
            return new ORC();
        }
    }
}
