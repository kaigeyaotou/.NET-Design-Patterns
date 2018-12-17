using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 一个普通的公开课学员,学习公开课
    /// </summary>
    public class StudentFree : AbstractStudent
    {
        public override void Study()
        {
            Console.WriteLine("{0} is a free student studying .net Free", base.Name);
        }
    }
}
