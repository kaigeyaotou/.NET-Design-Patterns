using System;

namespace BuilderPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            BuilderPattern builderPattern =new  BuilderPattern();
            builderPattern.NewGame();
            builderPattern.NewGameCount();
            Console.WriteLine($"按任意键退出。。。");
            Console.ReadLine();
        }
    }
}
