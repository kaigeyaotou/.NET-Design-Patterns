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
    /// 
    /// 懒汉式
    /// </summary>
    public class Singleton//<T> 泛型类型里面的静态字段，是随着不同的类型参数唯一的
    {
        /// <summary>
        /// 构造函数耗时耗资源
        /// </summary>
        private Singleton()//private  避免外部创建
        {
            long lResult = 0;
            for (int i = 0; i < 10000000; i++)
            {
                lResult += i;
            }
            Thread.Sleep(1000);
            Console.WriteLine("{0}被构造一次", this.GetType().Name);
        }

        private static volatile Singleton _Singleton = null;
        private static object Singleton_Lock = new object();

        public static Singleton CreateInstance()
        {
            if (_Singleton == null)//保证对象初始化之后，不在去等待锁
            {
                lock (Singleton_Lock)//保证只有一个线程进去
                {
                    //Thread.Sleep(1000);
                    Console.WriteLine("这里做了1s的锁的等待");
                    if (_Singleton == null)//保证只被实例化一次
                        _Singleton = new Singleton();
                }
            }
            return _Singleton;
        }//懒汉式  调用了方法才去构造




        //既然是单例，大家用的是同一个对象，用的是同一个方法，那还会并发吗  还有线程安全问题吗？
        //当然有，，单例不能解决线程冲突的  解决：加锁

        public int iTotal = 0;
        public void Show()
        {
            //lock
            this.iTotal++;
        }

    }
}
