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
    /// 产品簇
    /// 单一职责就是创建完整的产品簇
    /// 
    /// 继承抽象类后，必须显式的override父类的抽象方法
    /// </summary>
    public abstract class FactoryAbstract
    {
        public abstract IRace CreateRace();
        public abstract IArmy CreateArmy();
        public abstract IHero CreateHero();
        public abstract IResource CreateResource();

        //public abstract ILuck CreateLuck();
    }
}
