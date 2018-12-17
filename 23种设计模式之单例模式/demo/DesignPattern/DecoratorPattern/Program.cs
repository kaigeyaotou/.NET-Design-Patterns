using DecoratorPattern.Decorator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    ///  纵向关系：继承/实现
    ///  横向关系：依赖   关联 组合 聚合
    ///  
    ///  结构型设计模式：组合优于继承
    ///  
    /// 装饰器模式：AOP  静态AOP
    /// 组合+继承
    /// 
    /// 装饰器模式可以在程序运行的过程中，为对象动态增加功能
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("欢迎来到.net高级班vip课程，今天是设计模式的学习");
                {
                    AbstractStudent student = new StudentVip()
                    {
                        Id = 381,
                        Name = "候鸟"
                    };
                    //RegUser
                    //付费  要预习   
                    student.Study();
                    Console.WriteLine("****************************************");
                    //{
                    //    AbstractStudent studentDecorator = new BaseStudentDecorator();
                    //    studentDecorator.Study();// 学习课程，，在学习前加点东西：付费  要预习 
                    //}
                    {
                        //StudentPreviewDecorator studentDecorator = new StudentPreviewDecorator(student);
                        //AbstractStudent studentDecorator = new StudentPreviewDecorator(student);//换成抽象变量
                        //studentDecorator.Study();

                        //student = new StudentHomeworkDecorator(student);

                        student = new StudentPreviewDecorator(student);//原有变量
                        student = new StudentRegDecorator(student);
                        student = new StudentPayDecorator(student);

                        student = new StudentHomeworkDecorator(student);
                        student = new StudentCommentDecorator(student);


                        //student.Study();
                        //学习之后  巩固练习homework
                        //student = new StudentHomeworkDecorator(student);
                        student.Study();
                    }
                }
                //{
                //    AbstractStudent student = new StudentFree()
                //    {
                //        Id = 381,
                //        Name = "候鸟"
                //    };
                //    student.Study();
                //    Console.WriteLine("****************************************");

                //    student = new StudentPreviewDecorator(student);//原有变量
                //    student = new StudentPayDecorator(student);
                //    student.Study();
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
