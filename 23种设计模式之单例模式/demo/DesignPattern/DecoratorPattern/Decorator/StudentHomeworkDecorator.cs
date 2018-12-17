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
    public class StudentHomeworkDecorator : BaseStudentDecorator
    {
        public StudentHomeworkDecorator(AbstractStudent student)
            : base(student)//表示父类的构造函数
        {

        }

        public override void Study()
        {
            base.Study();

            Console.WriteLine("巩固练习");
           
        }
    }
}
