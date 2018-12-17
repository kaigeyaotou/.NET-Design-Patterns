using System;
using System.Collections.Generic;
using System.Text;

namespace Builder_Pattern_Console.Libs
{
	/// <summary>
	/// 具体建造者(Concrete Builder）--数据中心项目
	///     实现 Builder 接口，完成复杂产品的各个部件的具体创建方法
	///     
	///  Datacenter项目团队
	/// </summary>
	public class ConcreteBuilder_DatacenterProject : ZRProjectBuilder
	{
		public override void Execute_Code()
		{
			zrproject.Code = $"执行编码阶段：编码【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}

		public override void Execute_Design()
		{
			zrproject.Design = $"执行设计阶段：设计【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}

		public override void Execute_Maintenance()
		{
			zrproject.Maintenance = $"执行运维阶段：运维【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}

		public override void Execute_Release()
		{
			zrproject.Release = $"执行发布阶段：发布【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}

		public override void Execute_RequirementAnalysis()
		{
			zrproject.RequirementAnalysis = $"执行需求分析阶段：需求分析【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}

		public override void Execute_Test()
		{
			zrproject.Test = $"执行测试阶段：测试【{nameof(ConcreteBuilder_DatacenterProject)}】";
		}
	}
}
