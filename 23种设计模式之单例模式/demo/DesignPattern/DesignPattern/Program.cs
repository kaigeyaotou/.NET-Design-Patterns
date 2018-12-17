using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    /// <summary>
    /// 1 创建型设计模式
    /// 2 结构型设计模式
    /// 3 行为型设计模式
    /// 
    /// 设计模式：面向对象语言开发过程，遇到的一些常见的问题和场景的解决方案，
    /// 沉淀下来，就成了设计模式，，一种设计模式就是解决一类问题的，
    /// 没有什么设计模式是完美无缺的，通常还会有其他的毛病
    /// 23种设计模式模式  不包含简单工厂
    /// 
    /// 创建型设计模式：关注对象的创建  
    /// 单例&原型  三大工厂+建造者
    /// 
    /// 结构型设计模式：关注类与类之间的关系
    /// 继承--实现
    /// 聚合  组合  关联  依赖
    /// 组合优于继承
    /// 适配器  代理模式
    /// 
    /// 行为型设计模式：关注对象和行为的分离
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("学习设计模式");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
