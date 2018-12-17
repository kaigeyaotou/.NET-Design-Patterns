using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using FactoryPattern.War3.ServiceExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Factory
{
    /// <summary>
    /// 比如构造很复杂。。比如依赖其他对象
    /// 屏蔽变化
    /// </summary>
    public class FiveFactory : IFactory
    {
        public virtual IRace CreateRace()
        {

            //return new Five();
            return new Five(2, "New", 2);
        }
    }
}
