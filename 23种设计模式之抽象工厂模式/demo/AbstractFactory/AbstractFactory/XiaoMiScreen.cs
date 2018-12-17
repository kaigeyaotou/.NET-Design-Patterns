using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 小米手机屏幕
    /// </summary>
    public class XiaoMiScreen : Screen
    {
        public override void Print()
        {
            Console.WriteLine("小米手机屏幕！");
        }
    }
}
