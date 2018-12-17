using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern.Decorator
{
    /// <summary>
    /// 装饰器的基类
    /// 也是一个学员，继承了抽象类
    /// </summary>
    public class BaseStudentDecorator : AbstractStudent
    {
        private AbstractStudent _Student = null;//用了组合加override
        public BaseStudentDecorator(AbstractStudent student)
        {
            this._Student = student;
        }

        public override void Study()
        {
            this._Student.Study();
            //Console.WriteLine("****************");
            //基类装饰器必须是个空的行为  会重复
        }
    }
}
