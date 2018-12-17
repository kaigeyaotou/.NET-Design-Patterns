using System;
using System.Collections.Generic;
using System.Text;

namespace Builder_Pattern_Console.Libs
{
	/// <summary>
	/// 抽象建造者:
	///     产品对象的各个部件抽象接口
	///     通常还包含一个返回复杂产品的方法 getResult()
	///     
	/// 项目研发生命周期
	/// </summary>
	public abstract class ZRProjectBuilder
	{
		protected ZRProject zrproject = new ZRProject();

		public abstract void Execute_RequirementAnalysis();
		public abstract void Execute_Design();
		public abstract void Execute_Code();
		public abstract void Execute_Test();
		public abstract void Execute_Release();
		public abstract void Execute_Maintenance();

		public ZRProject GetExecuteResult()
		{
			return zrproject;
		}
	}
}
