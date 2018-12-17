using System;

namespace AbstractFactory
{
    /// <summary>
    /// 华为手机工厂类
    /// </summary>
    public class HuaWeiFactory : AbstractFactory
    {
        /// <summary>
        /// 生产华为手机屏幕
        /// </summary>
        /// <returns></returns>
        public override Screen CreateScreen()
        {
            return new HuaWeiScreen();
        }

        /// <summary>
        /// 生产华为手机主板
        /// </summary>
        /// <returns></returns>
        public override MotherBoard CreateMotherBoard()
        {
            return new HuaWeiMotherBoard();
        }
    }

    /// <summary>
    /// 华为手机屏幕
    /// </summary>
    public class HuaWeiScreen : Screen
    {
        public override void Print()
        {
            Console.WriteLine("华为手机屏幕！");
        }
    }

    /// <summary>
    /// 华为手机主板类
    /// </summary>
    public class HuaWeiMotherBoard : MotherBoard
    {
        public override void Print()
        {
            Console.WriteLine("华为手机主板！");
        }

    }
}
