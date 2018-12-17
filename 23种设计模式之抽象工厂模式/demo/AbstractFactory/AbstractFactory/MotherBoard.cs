using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 主板抽象类：提供每一品牌的主板的继承
    /// </summary>
    public abstract class MotherBoard
    {
        public abstract void Print();
    }
}
