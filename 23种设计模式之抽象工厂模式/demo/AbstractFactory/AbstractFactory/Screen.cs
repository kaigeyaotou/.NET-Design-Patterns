using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 屏幕抽象类：提供每一品牌的屏幕的继承
    /// </summary>
    public abstract class Screen
    {
        public abstract void Print();
    }
}
