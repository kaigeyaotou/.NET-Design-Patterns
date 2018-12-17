using AbstractFactory.Factory;
using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 抽象工厂
    /// 
    /// 倾斜的可扩展性设计
    /// 
    ///工厂+约束
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("想要玩一款游戏，必须4大元素备齐");

                {
                    IRace race = new Undead();
                    IArmy army = new UndeadArmy();
                    IHero hero = new UndeadHero();
                    IResource resource = new UndeadResource();
                    //1 对象转移，屏蔽细节，让使用者更轻松
                    //2 对象簇的工厂
                }
                {

                    //System.Data.SqlClient.SqlClientFactory
                    FactoryAbstract undeadFactory = new UndeadFactory();
                    IRace race = undeadFactory.CreateRace();// new Undead();
                    IArmy army = undeadFactory.CreateArmy();//new UndeadArmy();
                    IHero hero = undeadFactory.CreateHero();//new UndeadHero();
                    IResource resource = undeadFactory.CreateResource();//new UndeadResource();
                }
                {
                    FactoryAbstract humanFactory = new HumanFactory();
                    IRace race = humanFactory.CreateRace();
                    IArmy army = humanFactory.CreateArmy();
                    IHero hero = humanFactory.CreateHero();
                    IResource resource = humanFactory.CreateResource();
                }

                {
                    FactoryAbstract humanFactory = new ORCFactory();
                    IRace race = humanFactory.CreateRace();
                    IArmy army = humanFactory.CreateArmy();
                    IHero hero = humanFactory.CreateHero();
                    IResource resource = humanFactory.CreateResource();
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
