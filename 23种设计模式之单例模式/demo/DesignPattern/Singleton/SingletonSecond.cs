using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SingletonPattern
{
    /// <summary>
    /// 单例类：一个构造对象很耗时耗资源类型
    /// </summary>
    public class SingletonSecond
    {
        /// <summary>
        /// 构造函数耗时耗资源
        /// </summary>
        private SingletonSecond()
        {
            long lResult = 0;
            for (int i = 0; i < 10000000; i++)
            {
                lResult += i;
            }
            Thread.Sleep(1000);
            Console.WriteLine("{0}被构造一次", this.GetType().Name);
        }

        /// <summary>
        /// 静态构造函数:由CLR保证，程序第一次使用这个类型前被调用，且只调用一次
        /// 
        /// 写日志功能的文件夹检测
        /// XML配置文件
        /// </summary>
        static SingletonSecond()
        {
            _SingletonSecond = new SingletonSecond();
            Console.WriteLine("SingletonSecond 被启动");
        }


        private static SingletonSecond _SingletonSecond = null;
        public static SingletonSecond CreateInstance()
        {
            return _SingletonSecond;
        }//饿汉式  只要使用类就会被构造

        /// <summary>
        /// 原型模式:解决对象重复创建的问题
        /// 通过MemberwiseClone来clone新对象，避免重复创建
        /// </summary>
        /// <returns></returns>
        public static SingletonSecond CreateInstancePrototype()
        {
            SingletonSecond second = (SingletonSecond)_SingletonSecond.MemberwiseClone();
            return second;
        }



        public int iTotal = 0;
        public void Show()
        {
            this.iTotal++;
        }

    }
}
