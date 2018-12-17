using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 苹果手机主板
    /// </summary>
    public class AppleMotherBoard : MotherBoard
    {
        public override void Print()
        {
            Console.WriteLine("苹果手机主板！");
        }
    }
}
