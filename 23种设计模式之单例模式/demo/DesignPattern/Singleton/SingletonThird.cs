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
    public class SingletonThird
    {
        /// <summary>
        /// 构造函数耗时耗资源
        /// </summary>
        private SingletonThird()
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
        /// 静态字段：在第一次使用这个类之前，由CLR保证，初始化且只初始化一次
        /// </summary>
        private static SingletonThird _SingletonThird = new SingletonThird();//打印个日志
        public static SingletonThird CreateInstance()
        {
            return _SingletonThird;
        }//饿汉式  只要使用类就会被构造




        public void Show()
        {
            Console.WriteLine("这里是{0}.Show", this.GetType().Name);
        }

    }
}
