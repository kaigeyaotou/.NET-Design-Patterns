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
    /// 玩家
    /// </summary>
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void PlayHuman(Human human)
        {
            Console.WriteLine("******************************");
            Console.WriteLine("This is {0} Play War3.{1}", this.Name, human.GetType().Name);
            human.ShowKing();
        }

        public void PlayORC(ORC orc)
        {
            Console.WriteLine("******************************");
            Console.WriteLine("This is {0} Play War3.{1}", this.Name, orc.GetType().Name);
            orc.ShowKing();
        }

        public void PlayORC(Undead undead)
        {
            Console.WriteLine("******************************");
            Console.WriteLine("This is {0} Play War3.{1}", this.Name, undead.GetType().Name);
            undead.ShowKing();
        }

        /// <summary>
        /// 面向抽象
        /// </summary>
        /// <param name="race"></param>
        public void PlayWar3(IRace race)
        {
            Console.WriteLine("******************************");
            Console.WriteLine("This is {0} Play War3.{1}", this.Name, race.GetType().Name);
            race.ShowKing();
        }
        

    }
}
