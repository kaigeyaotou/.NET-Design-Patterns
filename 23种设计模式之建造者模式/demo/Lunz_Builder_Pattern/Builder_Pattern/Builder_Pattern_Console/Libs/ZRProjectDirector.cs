using System;
using System.Collections.Generic;
using System.Text;

namespace Builder_Pattern_Console.Libs
{
	/// <summary>
	/// 指挥者（Director）：
	///     调用建造者对象中的部件构造与装配方法完成复杂对象的创建
	///     不涉及具体产品的信息
	///    
	/// 项目经理
	/// </summary>
	public class ZRProjectDirector
	{
		private ZRProjectBuilder _builder;
		public ZRProjectDirector(ZRProjectBuilder builder)
		{
			_builder = builder;
		}

		public ZRProject BuldZRProject()
		{
			_builder.Execute_RequirementAnalysis();
			_builder.Execute_Design();
			_builder.Execute_Code();
			_builder.Execute_Test();
			_builder.Execute_Release();
			_builder.Execute_Maintenance();

			return _builder.GetExecuteResult();
		}

	}
}
