using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 小米手机工厂类
    /// </summary>
    public class XiaoMiFactory : AbstractFactory
    {
        /// <summary>
        /// 生产小米手机屏幕
        /// </summary>
        /// <returns></returns>
        public override Screen CreateScreen()
        {
            return new XiaoMiScreen();
        }

        /// <summary>
        /// 生产小米手机主板
        /// </summary>
        /// <returns></returns>
        public override MotherBoard CreateMotherBoard()
        {
            return new XiaoMiMotherBoard();
        }
    }
}
