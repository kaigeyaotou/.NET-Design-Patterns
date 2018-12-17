using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 抽象工厂类：提供创建不同品牌的手机屏幕和手机主板
    /// </summary>
    public abstract class AbstractFactory
    {
        //工厂生产屏幕
        public abstract Screen CreateScreen();
        //工厂生产主板
        public abstract MotherBoard CreateMotherBoard();
    }
}
