using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 苹果手机工厂
    /// </summary>
    public class AppleFactory : AbstractFactory
    {
        /// <summary>
        /// 生产苹果手机屏幕
        /// </summary>
        /// <returns></returns>
        public override Screen CreateScreen()
        {
            return new AppleScreen();
        }

        /// <summary>
        /// 生产苹果手机主板
        /// </summary>
        /// <returns></returns>
        public override MotherBoard CreateMotherBoard()
        {
            return new AppleMotherBoard();
        }
    }
}
