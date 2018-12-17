using System;
using System.Collections.Generic;
using System.Text;

namespace Builder_Pattern_Console.Libs
{
	/// <summary>
	/// 产品角色-中瑞项目
	///          一个具体的产品对象
	/// 包含多个组成部件：
	///       需求分析
	///       设计
	///       编码
	///       测试
	///       发布
	///       运维
	/// </summary>
	public class ZRProject
	{
		public string RequirementAnalysis { get; set; }
		public string Design { get; set; }
		public string Code { get; set; }
		public string Test { get; set; }
		public string Release { get; set; }
		public string Maintenance { get; set; }

		/// <summary>
		/// 交付项目
		/// </summary>
		public string DeliveryProject()
		{
			return RequirementAnalysis + Environment.NewLine
				+ Design + Environment.NewLine
				+ Code + Environment.NewLine
				+ Test + Environment.NewLine
				+ Release + Environment.NewLine
				+ Maintenance + Environment.NewLine
				+ "，项目开发完成，交付客户";


		}
	}
}