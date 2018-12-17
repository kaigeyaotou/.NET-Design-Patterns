using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingletonPattern
{
    /// <summary>
    /// 单例模式
    /// 
    /// 饿汉式 ：第一时间创建实例
    /// 懒汉式 ：需要才创建实例
    /// 
    /// 单例模式会长期持有一个对象，不会释放
    /// 普通实例使用完后释放
    /// 
    /// 单例可以只构造一次，提升性能(如果构造函数耗性能)
    /// 
    /// 单例就是保证类型只有一个实例：计数器/数据库连接池
    /// 
    /// 程序中某个对象，只有一个实例
    /// </summary>
    class Program
    {
        /// <summary>
        /// 静态字段在程序进程只有一个
        /// </summary>
        //public static Singleton singleton = new Singleton();
        static void Main(string[] args)
        {
            try
            {
                SingletonSecond second = SingletonSecond.CreateInstance();
                SingletonThird third = SingletonThird.CreateInstance();


                //Singleton singleton = new Singleton();
                //对象的重用 

                TaskFactory taskFactory = new TaskFactory();
                List<Task> taskList = new List<Task>();
                //for (int i = 0; i < 50000; i++)
                //{
                //    taskList.Add(taskFactory.StartNew(() =>
                //    {
                //        Singleton singleton = Singleton.CreateInstance();// new Singleton();
                //        singleton.Show();//多线程去运行同一个实例的同一个方法
                //    }));
                //}
                for (int i = 0; i < 50000; i++)
                {
                    taskList.Add(taskFactory.StartNew(() =>
                    {
                        SingletonSecond singleton = SingletonSecond.CreateInstancePrototype();// new Singleton();
                        singleton.Show();
                    }));
                }

                Task.WaitAll(taskList.ToArray());
                Console.WriteLine("第一轮全部完成");

                //for (int i = 0; i < 5; i++)
                //{
                //    taskList.Add(taskFactory.StartNew(() =>
                //    {
                //        Singleton singleton = Singleton.CreateInstance();// new Singleton();
                //        //singleton.Show();
                //    }));
                //}

                //0  1  50000  还是接近50000
                Console.WriteLine(SingletonSecond.CreateInstancePrototype().iTotal);

                //for (int i = 0; i < 5; i++)
                //{
                //    taskFactory.StartNew(() =>
                //    {
                //        Singleton singleton = Singleton.CreateInstance();// new Singleton();
                //        singleton.Show();
                //    });
                //}




                //OtherClass.Show();
                //OtherClass.Show();
                //OtherClass.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
