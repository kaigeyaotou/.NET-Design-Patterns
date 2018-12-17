using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 一个普通的vip学员,学习vip课程
    /// </summary>
    public class StudentVip : AbstractStudent
    {
        /// <summary>
        /// 付费  上课前要预习   
        /// 上课学习
        /// </summary>
        public override void Study()
        {
            Console.WriteLine("{0} is a vip student studying .net Vip", base.Name);
        }
    }
}
