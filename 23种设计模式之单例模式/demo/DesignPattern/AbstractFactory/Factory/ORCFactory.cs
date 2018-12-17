using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory.Factory
{
    /// <summary>
    /// 一个工厂负责一些产品的创建
    /// </summary>
    public class ORCFactory : FactoryAbstract
    {
        public override IRace CreateRace()
        {
            return new ORC();
        }

        public override IArmy CreateArmy()
        {
            return new ORCArmy();
        }
        public override IHero CreateHero()
        {
            return new ORCHero();
        }
        public override IResource CreateResource()
        {
            return new ORCResource();
        }
    }
}
