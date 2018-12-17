using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern.Decorator
{

    /// <summary>
    /// 父类是BaseStudentDecorator，爷爷类AbstractStudent
    /// </summary>
    public class StudentPreviewDecorator : BaseStudentDecorator
    {
        public StudentPreviewDecorator(AbstractStudent student)
            : base(student)//表示父类的构造函数
        {

        }

        public override void Study()
        {
            Console.WriteLine("预习");
            base.Study();
        }
    }
}
