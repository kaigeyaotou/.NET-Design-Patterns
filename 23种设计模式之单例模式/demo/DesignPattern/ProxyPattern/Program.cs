using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPattern
{
    /// <summary>
    /// 代理模式
    /// 
    /// 在不修改RealSubject前提下，插入功能
    /// 
    /// 包一层:没有什么技术问题是包一层不能解决的，如果有，就再包一层
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                {
                    Console.WriteLine("***********Real**************");
                
                    ISubject subject = new RealSubject();//持有资源 /数据库链接

                    Console.WriteLine("do something else...");
                    Thread.Sleep(500);
                    Console.WriteLine("do something else...");
                    Console.WriteLine("do something else...");
                    Console.WriteLine("do something else...");
                    //subject.GetSomething();
                    subject.DoSomething();
                }
                {
                    Console.WriteLine("***********Proxy1**************");
                    ISubject subject = new ProxySubject();

                    Console.WriteLine("do something else...");
                    Thread.Sleep(500);
                    Console.WriteLine("do something else...");
                    Console.WriteLine("do something else...");
                    Console.WriteLine("do something else...");

                    //subject.GetSomething();//真的需要的时候，才去   持有资源 /数据库链接
                    subject.DoSomething();
                }
                {
                    Console.WriteLine("***********Proxy1**************");
                    ISubject subject = new ProxySubject();

                    subject.GetSomething();
                    subject.DoSomething();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
