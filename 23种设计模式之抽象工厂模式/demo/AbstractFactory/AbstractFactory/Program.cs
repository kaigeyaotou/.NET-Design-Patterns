using System;

namespace AbstractFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            //小米工厂生产小米手机的屏幕和主板
            Console.WriteLine("小米工厂:");
            AbstractFactory xiaomiFactory = new XiaoMiFactory();
            Screen xiaomiScreen = xiaomiFactory.CreateScreen();
            xiaomiScreen.Print();
            MotherBoard xiaomiMotherBoard = xiaomiFactory.CreateMotherBoard();
            xiaomiMotherBoard.Print();

            Console.WriteLine();

            //苹果工厂生产苹果手机屏幕和主板
            Console.WriteLine("苹果工厂:");
            AbstractFactory appleFactory = new AppleFactory();
            Screen appleScreen = appleFactory.CreateScreen();
            appleScreen.Print();
            MotherBoard appleMotherBoard = appleFactory.CreateMotherBoard();
            appleMotherBoard.Print();

            Console.WriteLine();

            //华为工厂生产苹果手机屏幕和主板
            Console.WriteLine("华为工厂:");
            AbstractFactory huaWeiFactory = new HuaWeiFactory();
            Screen huaWeiScreen = huaWeiFactory.CreateScreen();
            appleScreen.Print();
            MotherBoard huaWeiMotherBoard = appleFactory.CreateMotherBoard();
            appleMotherBoard.Print();

            Console.Read();
        }
    }
}
