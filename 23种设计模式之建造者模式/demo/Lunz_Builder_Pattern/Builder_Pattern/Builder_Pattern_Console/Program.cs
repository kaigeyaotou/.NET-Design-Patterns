using Builder_Pattern_Console.Libs;
using System;

namespace Builder_Pattern_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("*************************************************");
			Console.WriteLine("***************设计模式-建造者模式***************");
			Console.WriteLine("*************************************************");
			Console.WriteLine(Environment.NewLine + "1：开发lunz#项目  2：开发数据中心项目 0：退出");

			string project = string.Empty;
			while (true)
			{
				Console.WriteLine("请选择要执行的操作：");
				project = Console.ReadLine();
				switch (project)
				{
					case "1":
						//客户要开发Lunzsharp项目
						ZRProjectBuilder lunzsharp_builder = new ConcreteBuilder_LunzsharpProject();
						//客户让项目经理lunzsharp_director开发自己的项目
						ZRProjectDirector lunzsharp_director = new ZRProjectDirector(lunzsharp_builder);
						//项目经理lunzsharp_director带领团队进行开发
						ZRProject lunzsharp_project = lunzsharp_director.BuldZRProject();
						//项目经理lunzsharp_director交付开发完成的产品
						Console.WriteLine(lunzsharp_project.DeliveryProject());
						break;
					case "2":
						//客户要开发Datacenter项目
						ZRProjectBuilder dc_builder = new ConcreteBuilder_DatacenterProject();
						//客户让项目经理dc_director开发自己的项目
						ZRProjectDirector dc_director = new ZRProjectDirector(dc_builder);
						//项目经理dc_director带领团队进行开发
						ZRProject dc_project = dc_director.BuldZRProject();
						//项目经理dc_director交付开发完成的产品
						Console.WriteLine(dc_project.DeliveryProject());
						break;
					case "0":
						Environment.Exit(0);
						break;
					default:
						Console.WriteLine("无法识别的操作类型,请重新选择");
						break;
				}
				Console.WriteLine("*************************************************");
				Console.WriteLine(Environment.NewLine + "1：开发lunz#项目  2：开发数据中心项目 0：退出");
			}
		}
	}
}
