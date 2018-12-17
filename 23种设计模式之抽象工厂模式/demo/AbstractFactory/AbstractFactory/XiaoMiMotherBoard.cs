using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 小米手机主板类
    /// </summary>
    public class XiaoMiMotherBoard : MotherBoard
    {
        public override void Print()
        {
            Console.WriteLine("小米手机主板！");
        }

    }
}
