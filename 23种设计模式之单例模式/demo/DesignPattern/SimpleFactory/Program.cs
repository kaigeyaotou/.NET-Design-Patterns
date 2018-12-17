using FactoryPattern.War3.Interface;
using FactoryPattern.War3.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFactory
{
    /// <summary>
    /// 简单工厂
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Player player = new Player()
                {
                    Id = 123,
                    Name = "候鸟"
                };
                #region
                {
                    Human human = new Human();
                    player.PlayHuman(human);

                    player.PlayWar3(human);
                }
                {
                    ORC orc = new ORC();
                    player.PlayORC(orc);

                    player.PlayWar3(orc);
                }
                {
                    Undead undead = new Undead();
                    player.PlayWar3(undead);
                }
                {
                    NE ne = new NE();
                    player.PlayWar3(ne);
                }
                #endregion

                {
                    Human human = new Human();//1 到处都是细节
                    player.PlayWar3(human);
                }
                {
                    IRace human = new Human();//2 左边是抽象  右边是细节
                    player.PlayWar3(human);
                }
                {
                    IRace human = ObjectFactory.CreateRace(RaceType.Human); //new Human();//3 没有细节  细节被转移
                    player.PlayWar3(human);
                }
                {
                    IRace undead = ObjectFactory.CreateRace(RaceType.Undead); //new Human();//3 没有细节 细节被转移
                    player.PlayWar3(undead);
                }
                Console.WriteLine("*********************CreateRaceConfig*****************");
                {
                    IRace undead = ObjectFactory.CreateRaceConfig(); //new Human();//4 可配置
                    player.PlayWar3(undead);
                }
                Console.WriteLine("**************************************");
                {
                    IRace undead = ObjectFactory.CreateRaceConfigReflection(); //5 可配置可扩展
                    player.PlayWar3(undead);
                }
                //IOC
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
