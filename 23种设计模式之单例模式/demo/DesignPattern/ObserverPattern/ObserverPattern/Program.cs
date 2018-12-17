using ObserverPattern.Observer;
using ObserverPattern.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    /// <summary>
    /// 观察者模式
    /// 对象和行为的分离
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                {
                    Cat cat = new Cat();
                    cat.Miao();
                }
                Console.WriteLine("*******************************");
                {
                    Cat cat = new Cat();

                    cat.Add(new Baby());
                    cat.Add(new Brother());
                    cat.Add(new Chicken());
                    cat.Add(new Dog());
                    cat.Add(new Father());
                    cat.Add(new Mother());
                    cat.Add(new Mouse());
                    cat.Add(new Neighbor());
                    cat.Add(new Stealer());

                    cat.MiaoObserver();
                }
                Console.WriteLine("*******************************");
                {
                    Cat cat = new Cat();

                    //cat.Add(new Baby());
                    cat.Add(new Chicken());
                    cat.Add(new Brother());
                    cat.Add(new Dog());
                    cat.Add(new Father());
                    cat.Add(new Mother());
                    cat.Add(new Mouse());
                    cat.Add(new Neighbor());
                    cat.Add(new Stealer());

                    cat.MiaoObserver();
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
