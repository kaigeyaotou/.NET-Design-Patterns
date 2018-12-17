using FactoryMethod.Factory;
using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using FactoryPattern.War3.ServiceExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    /// <summary>
    /// 工厂方法
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                {
                    Human human = new Human();//1 到处都是细节
                }
                {
                    IRace human = new Human();//2 左边是抽象  右边是细节
                }
                {
                    //human.
                    IFactory factory = new HumanFactory();//包一层
                    IRace race = factory.CreateRace();
                }
                {
                    //Undead
                    IFactory factory = new UndeadFactory();
                    IRace race = factory.CreateRace();
                }
                {
                    IRace five = new Five();//修改
                }

                {
                    //five
                    IFactory factory = new FiveFactory();
                    IRace race = factory.CreateRace();
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
